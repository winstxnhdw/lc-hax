using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine.EventSystems;
using ZLinq;

static class Chat {
    internal static event Action<string>? OnExecuteCommandAttempt;

    static CancellationTokenSource CommandCancellation { get; set; } = new();

    static ValueEnumerable<ZLinq.Linq.ArrayWhere<Type>, Type> CommandTypes { get; } =
        Assembly.GetExecutingAssembly()
                .GetTypes()
                .AsValueEnumerable()
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
        Chat.CommandTypes.Where(type => type.GetCustomAttribute<PrivilegedCommandAttribute>() is not null).ToDictionary(
            type => type.GetCustomAttribute<PrivilegedCommandAttribute>().Syntax,
            type => (ICommand)new PrivilegedCommand((ICommand)Activator.CreateInstance(type))
        );

    static Chat() {
        GameListener.OnShipLeave += Chat.CancelCommands;
        GameListener.OnGameEnd += Chat.CancelCommands;
    }

    internal static void Clear() {
        Helper.HUDManager?.AddTextToChatOnServer(
            $"</color>\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n<color=#FFFFFF00>"
        );
    }

    internal static void Print(string name, string? message, bool isSystem = false) {
        if (string.IsNullOrWhiteSpace(message)) return;
        if (Helper.HUDManager is not { } hudManager) return;

        hudManager.AddChatMessage(message, name);

        if (!isSystem && hudManager.localPlayer.isTypingChat) {
            hudManager.localPlayer.isTypingChat = false;
            hudManager.typingIndicator.enabled = false;
            hudManager.chatTextField.text = "";
            hudManager.PingHUDElement(hudManager.Chat, 1.0f, 1.0f, 0.2f);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    internal static void Print(string? message) => Chat.Print("SYSTEM", message, true);

    internal static void Print(string? message, params string[] args) =>
        Chat.Print($"{message}\n{string.Join('\n', args)}");

    static void CancelCommands() {
        using CancellationTokenSource previousCancellation = Chat.CommandCancellation;
        Chat.CommandCancellation = new();
        previousCancellation.Cancel();
    }

    internal static async void ExecuteCommand(string commandString) {
        CancellationToken cancellationToken = Chat.CommandCancellation.Token;

        try {
            Chat.Print("USER", commandString);
            Chat.OnExecuteCommandAttempt?.Invoke(commandString);
            Arguments args = commandString[1..].Split(' ');

            ICommand? command =
                Chat.Commands.GetValue(args[0]) ??
                Chat.PrivilegeCommands.GetValue(args[0]) ??
                Chat.DebugCommands.GetValue(args[0]);

            if (command is null) {
                Chat.Print("The command is not found!");
                return;
            }

            await command.Execute(args[1..], cancellationToken);
        }

        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
        catch (Exception exception) {
            Logger.Write(exception.ToString());
        }
    }
}
