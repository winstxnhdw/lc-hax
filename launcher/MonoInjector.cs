using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Iced.Intel;
using Microsoft.Win32.SafeHandles;
using PeNet;
using static Iced.Intel.AssemblerRegisters;

static class MonoInjector {
    internal static void Inject(Process game, byte[] payload) {
        ProcessModule mono = game.Modules.Cast<ProcessModule>().First(module => module.ModuleName == "mono-2.0-bdwgc.dll");
        Dictionary<string, ulong> exports = new PeFile(mono.FileName).ExportedFunctions!
            .Where(function => function.Name is not null)
            .ToDictionary(function => function.Name!, function => unchecked((ulong)mono.BaseAddress) + function.Address);

        byte[] data = [.. new byte[40], .. "lc-hax.dll\0Hax\0Loader\0Load\0"u8, .. payload];
        data[0] = 1;
        BitConverter.GetBytes(payload.Length).CopyTo(data, 32);

        using SafeProcessHandle process = NativeMethods.OpenProcess(0x043A, false, game.Id);
        MonoInjector.Check(!process.IsInvalid);
        nint memory = NativeMethods.VirtualAllocEx(process, 0, unchecked((nuint)(4096 + data.Length)), 0x3000, 0x04);
        MonoInjector.Check(memory != 0);
        bool release = true;

        try {
            byte[] code = MonoInjector.BuildCode(exports, unchecked((ulong)memory));
            MonoInjector.Check(NativeMethods.WriteProcessMemory(process, memory, code, unchecked((nuint)code.Length), out _));
            MonoInjector.Check(NativeMethods.WriteProcessMemory(process, memory + 4096, data, unchecked((nuint)data.Length), out _));
            MonoInjector.Check(NativeMethods.VirtualProtectEx(process, memory, 4096, 0x20, out _));
            MonoInjector.Check(NativeMethods.FlushInstructionCache(process, memory, unchecked((nuint)code.Length)));
            using SafeWaitHandle thread = NativeMethods.CreateRemoteThread(process, 0, 0, memory, memory + 4096, 0, out _);
            MonoInjector.Check(!thread.IsInvalid);

            if (NativeMethods.WaitForSingleObject(thread, 15000) != 0) {
                release = false;
                throw new InvalidOperationException("The loader did not finish. Restart the game before retrying.");
            }

            if (!NativeMethods.GetExitCodeThread(thread, out uint result)) {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            if (result != 0) {
                throw new InvalidOperationException($"Could not load lc-hax (0x{result:X}).");
            }
        }

        finally {
            if (release) {
                _ = NativeMethods.VirtualFreeEx(process, memory, 0, 0x8000);
            }
        }
    }

    static byte[] BuildCode(Dictionary<string, ulong> exports, ulong address) {
        Assembler code = new(64);
        Label cleanup = code.CreateLabel();
        Label detach = code.CreateLabel();

        code.push(rbx);
        code.sub(rsp, 32);
        code.mov(rbx, rcx);
        code.call(exports["mono_get_root_domain"]);
        MonoInjector.Next(code, cleanup);
        code.call(exports["mono_thread_attach"]);
        code.mov(__[rbx + 8], rax);
        MonoInjector.Next(code, cleanup);

        code.lea(rcx, __[rbx + 67]);
        code.mov(edx, __[rbx + 32]);
        code.mov(r8d, 1);
        code.lea(r9, __[rbx + 4]);
        code.call(exports["mono_image_open_from_data"]);
        code.mov(__[rbx + 16], rax);
        MonoInjector.Next(code, cleanup);
        code.lea(rdx, __[rbx + 40]);
        code.lea(r8, __[rbx + 4]);
        code.call(exports["mono_assembly_load_from"]);
        MonoInjector.Next(code, cleanup);
        code.call(exports["mono_assembly_get_image"]);
        MonoInjector.Next(code, cleanup);
        code.lea(rdx, __[rbx + 51]);
        code.lea(r8, __[rbx + 55]);
        code.call(exports["mono_class_from_name"]);
        MonoInjector.Next(code, cleanup);
        code.lea(rdx, __[rbx + 62]);
        code.xor(r8d, r8d);
        code.call(exports["mono_class_get_method_from_name"]);
        MonoInjector.Next(code, cleanup);
        code.xor(edx, edx);
        code.xor(r8d, r8d);
        code.lea(r9, __[rbx + 24]);
        code.call(exports["mono_runtime_invoke"]);
        code.cmp(__qword_ptr[rbx + 24], 0);
        code.jne(cleanup);
        code.mov(__dword_ptr[rbx], 0);

        code.Label(ref cleanup);
        code.mov(rcx, __[rbx + 16]);
        code.test(rcx, rcx);
        code.jz(detach);
        code.call(exports["mono_image_close"]);
        code.Label(ref detach);
        code.mov(rcx, __[rbx + 8]);
        code.call(exports["mono_thread_detach"]);
        code.mov(eax, __[rbx]);
        code.add(rsp, 32);
        code.pop(rbx);
        code.ret();

        byte[] buffer = new byte[4096];
        using MemoryStream stream = new(buffer);
        _ = code.Assemble(new StreamCodeWriter(stream), address);
        return buffer;
    }

    static void Next(Assembler code, Label cleanup) {
        code.test(rax, rax);
        code.jz(cleanup);
        code.mov(rcx, rax);
    }

    static void Check(bool success) {
        if (success) return;
        throw new Win32Exception(Marshal.GetLastWin32Error());
    }
}
