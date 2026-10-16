using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

Console.WriteLine("Waiting for Lethal Company to launch..");
Process? process;

while ((process = Process.GetProcessesByName("Lethal Company").SingleOrDefault()) is null) {
    Thread.Sleep(1000);
}

using Process game = process;
_ = game.WaitForInputIdle();
using Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("Launcher.Payload.lc-hax.dll")!;
byte[] payload = new byte[resource.Length];
resource.ReadExactly(payload);
Console.WriteLine("Loading lc-hax...");
MonoInjector.Inject(game, payload);
Console.WriteLine("lc-hax loaded.");

if (!Console.IsInputRedirected && !Console.IsOutputRedirected) {
    Console.WriteLine("Press any key to close.");
    _ = Console.ReadKey(intercept: true);
}
