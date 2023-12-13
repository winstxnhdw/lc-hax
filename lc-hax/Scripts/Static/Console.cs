using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Hax;

public static class Console {
    static Dictionary<string, ICommand> Commands { get; } = new() {
        { "/god", new GodCommand() },
        { "/shovel", new ShovelCommand() },
        { "/tp", new TeleportCommand() },
        { "/money", new MoneyCommand() },
        { "/unlock", new UnlockCommand() },
        { "/kill", new KillCommand() },
        { "/lock", new LockCommand() },
        { "/players", new PlayersCommand() },
        { "/xyz", new LocationCommand() },
        { "/home", new HomeCommand() },
        { "/end", new EndCommand() },
        { "/timescale", new TimescaleCommand() },
        { "/explode", new ExplodeCommand() },
        { "/noise", new NoiseCommand() },
        { "/random", new RandomCommand() },
        { "/stun", new StunCommand() },
        { "/stunclick", new StunOnClickCommand() },
        { "/heal", new HealCommand() },
        { "/ct", new ChibakuTenseiCommand() },
        { "/pumpkin", new PumpkinCommand() },
    };

    public static void Print(string name, string? message) {
        if (string.IsNullOrWhiteSpace(message) || !Helper.HUDManager.IsNotNull(out HUDManager hudManager)) return;

        _ = Reflector.Target(hudManager)?
                     .InvokeInternalMethod("AddChatMessage", message, name);

        if (hudManager.localPlayer.isTypingChat) {
            hudManager.localPlayer.isTypingChat = false;
            hudManager.typingIndicator.enabled = false;
            hudManager.chatTextField.text = "";
            hudManager.PingHUDElement(hudManager.Chat, 1.0f, 1.0f, 0.2f);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public static void ExecuteCommand(string command) {
        Console.Print("USER", command);
        Console.ExecuteCommand(command.Split(' '));
    }

    public static void ExecuteCommand(string[] args) {
        if (args.Length is 0) {
            Helper.PrintSystem("Usage: /<command> <args>");
            return;
        }

        if (!Console.Commands.ContainsKey(args[0])) {
            Helper.PrintSystem("Command not found!");
            return;
        }

        Console.Commands[args[0]].Execute(args[1..]);
    }
}



