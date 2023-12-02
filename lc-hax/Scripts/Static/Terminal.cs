using System.Collections.Generic;
using System.Linq;

namespace Hax;

public static class Terminal {
    static Dictionary<string, ICommand> Commands { get; } = new() {
        { "/god", new GodCommand() },
        { "/shovel", new ShovelCommand() },
        { "/tp", new TeleportCommand()}
    };

    static HUDManager? HUDManager { get; } = HaxObjects.Instance?.HUDManager.Object;

    static Reflector? hudManagerReflector = Terminal.HUDManager == null ? null : Reflector.Target(Terminal.HUDManager);

    static Reflector? HUDManagerReflector {
        get {
            if (Terminal.HUDManager == null) return null;

            hudManagerReflector ??= Reflector.Target(Terminal.HUDManager);
            return hudManagerReflector;
        }
    }

    public static void Print(string name, string message) {
        if (Terminal.HUDManager == null) return;

        _ = Terminal.HUDManagerReflector?.InvokeInternalMethod("AddChatMessage", message, name);

        if (Terminal.HUDManager.localPlayer.isTypingChat) {
            Terminal.HUDManager.localPlayer.isTypingChat = false;
            Terminal.HUDManager.typingIndicator.enabled = false;
            Terminal.HUDManager.chatTextField.text = "";
        }
    }

    public static void ExecuteCommand(string command) {
        Terminal.Print("USER", command);
        Terminal.ExecuteCommand(command.Split(' '));
    }

    public static void ExecuteCommand(string[] args) {
        if (args.Length < 1) {
            Terminal.Print("SYSTEM", "Usage: /<command> <args>");
            return;
        }

        if (!Terminal.Commands.ContainsKey(args[0])) {
            Terminal.Print("SYSTEM", "Command not found!");
            return;
        }

        Terminal.Commands[args[0]].Execute(args.Skip(1).ToArray());
    }
}



