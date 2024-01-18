using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.EventSystems;

namespace Hax;

public static class Chat {
    public static event Action<string>? onExecuteCommandAttempt;

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
                type => (ICommand)new Debug((ICommand)Activator.CreateInstance(type))
            );

    public static void Print(string name, string? message, bool isSystem = false) {
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

    public static void Print(string? message) {
        Chat.Print("SYSTEM", message, true);
    }

    public static void ExecuteCommand(string command) {
        Chat.Print("USER", command);
        Chat.onExecuteCommandAttempt?.Invoke(command);
        Chat.ExecuteCommand(command.Split(' '));
    }

    public static void ExecuteCommand(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /<command> <args>");
            return;
        }

        if (Chat.Commands.TryGetValue(args[0], out ICommand command)) {
            command.Execute(args[1..]);
            return;
        }

        if (Chat.DebugCommands.TryGetValue(args[0], out ICommand debugCommand)) {
            debugCommand.Execute(args[1..]);
            return;
        }

        Chat.Print("Command not found!");
    }
}



