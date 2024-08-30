using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Hax;
using GameNetcodeStuff;

static class Chat {
    internal static event Action<string>? OnExecuteCommandAttempt;

    static IEnumerable<Type> CommandTypes { get; } =
        Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => typeof(ICommand).IsAssignableFrom(type));

    static Dictionary<string, ICommand> Commands { get; } =
        Chat.CommandTypes.Where(type => type.GetCustomAttribute<CommandAttribute>() is not null).ToDictionary(
            type => type.GetCustomAttribute<CommandAttribute>().Syntax,
            type => (ICommand)Activator.CreateInstance(type)
        );

    static Dictionary<string, ICommand> DebugCommands { get; } =
        Chat.CommandTypes.Where(type => type.GetCustomAttribute<DebugCommandAttribute>() is not null).ToDictionary(
            type => type.GetCustomAttribute<DebugCommandAttribute>().Syntax,
            type => (ICommand)new DebugCommand((ICommand)Activator.CreateInstance(type))
        );

    static Dictionary<string, ICommand> PrivilegeCommands { get; } =
        Chat.CommandTypes.Where(type => type.GetCustomAttribute<HostCommandAttribute>() is not null).ToDictionary(
            type => type.GetCustomAttribute<HostCommandAttribute>().Syntax,
            type => (ICommand)new PrivilegedCommand((ICommand)Activator.CreateInstance(type))
        );

    internal static void Clear() {
        Helper.HUDManager?.AddTextToChatOnServer(
            $"</color>\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n<color=#FFFFFF00>"
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

    internal static void Print(string? message, params string[] args) => Chat.Print($"{message}\n{string.Join('\n', args)}");

    public static void Announce(string announcement, bool keepHistory = false) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (Helper.HUDManager is not HUDManager hudManager) return;

        string? actualHistory = string.Join('\n', hudManager.ChatMessageHistory.Where(message =>
            !message.StartsWith("<color=#FF0000>USER</color>: <color=#FFFF00>'") &&
            !message.StartsWith("<color=#FF0000>SYSTEM</color>: <color=#FFFF00>'")
        ));

        string chatText = keepHistory ? $"{actualHistory}\n<color=#7069ff>{announcement}</color>" : announcement;

        hudManager.AddTextToChatOnServer(
            $"</color>\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n{chatText}<color=#FFFFFF00>",
            (int)player.playerClientId
        );
    }

    internal static void ExecuteCommand(string commandString) {
        Chat.Print("USER", commandString);
        Chat.OnExecuteCommandAttempt?.Invoke(commandString);
        StringArray args = commandString[1..].Split(' ');

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
