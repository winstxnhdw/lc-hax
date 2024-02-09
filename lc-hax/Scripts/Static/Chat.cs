using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Hax;

internal static class Chat {
    internal static event Action<string>? OnExecuteCommandAttempt;

    static Dictionary<string, ICommand> Commands { get; } =
        Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(ICommand).IsAssignableFrom(type))
            .Where(type => type.GetCustomAttribute<CommandAttribute>() is not null)
            .ToDictionary(
                type => type.GetCustomAttribute<CommandAttribute>().Syntax,
                type => (ICommand)Activator.CreateInstance(type)
            );

    static Dictionary<string, ICommand> DebugCommands { get; } =
        Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(ICommand).IsAssignableFrom(type))
            .Where(type => type.GetCustomAttribute<DebugCommandAttribute>() is not null)
            .ToDictionary(
                type => type.GetCustomAttribute<DebugCommandAttribute>().Syntax,
                type => (ICommand)new DebugCommand((ICommand)Activator.CreateInstance(type))
            );

    static Dictionary<string, ICommand> PrivilegeCommands { get; } =
        Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(ICommand).IsAssignableFrom(type))
            .Where(type => type.GetCustomAttribute<PrivilegedCommandAttribute>() is not null)
            .ToDictionary(
                type => type.GetCustomAttribute<PrivilegedCommandAttribute>().Syntax,
                type => (ICommand)new PrivilegedCommand((ICommand)Activator.CreateInstance(type))
            );

    internal static void Clear() {
        Helper.HUDManager?.AddTextToChatOnServer(
            $"</color>\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n<color=#FFFFFF00>",
            -1
        );
    }

    internal static void Print(string name, string? message, bool isSystem = false) {
        if (string.IsNullOrWhiteSpace(message) || Helper.HUDManager is not HUDManager hudManager) return;
        _ = hudManager.Reflect().InvokeInternalMethod("AddChatMessage", message, name);

        if (!isSystem && hudManager.localPlayer.isTypingChat) {
            hudManager.localPlayer.isTypingChat = false;
            hudManager.typingIndicator.enabled = false;
            hudManager.chatTextField.text = "";
            hudManager.PingHUDElement(hudManager.Chat, 1.0f, 1.0f, 0.2f);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    internal static void Print(string? message) => Chat.Print("SYSTEM", message, true);

    internal static void ExecuteCommand(string command) {
        Chat.Print("USER", command);
        Chat.OnExecuteCommandAttempt?.Invoke(command);
        Chat.ExecuteCommand(command.Split(' '));
    }

    internal static void ExecuteCommand(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /<command> <args>");
            return;
        }

        ICommand? command =
            Chat.Commands.GetValue(args[0]) ??
            Chat.PrivilegeCommands.GetValue(args[0]) ??
            Chat.DebugCommands.GetValue(args[0]);

        if (command is null) {
            Chat.Print("The command is not found!");
            return;
        }

        command.Execute(args[1..]);
    }
}
