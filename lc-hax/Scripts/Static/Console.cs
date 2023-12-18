using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Hax;

public static class Console {
    static Dictionary<string, ICommand> Commands { get; } = new() {
        { "/god", new GodCommand() },
        { "/end", new EndCommand() },
        { "/hate", new HateCommand() },
        { "/kill", new KillCommand() },
        { "/lock", new LockCommand() },
        { "/home", new HomeCommand() },
        { "/stun", new StunCommand() },
        { "/heal", new HealCommand() },
        { "/wall", new WallCommand() },
        { "/tp", new TeleportCommand() },
        { "/noise", new NoiseCommand() },
        { "/money", new MoneyCommand() },
        { "/xyz", new LocationCommand() },
        { "/shovel", new ShovelCommand() },
        { "/unlock", new UnlockCommand() },
        { "/random", new RandomCommand() },
        { "/pumpkin", new PumpkinCommand() },
        { "/players", new PlayersCommand() },
        { "/explode", new ExplodeCommand() },
        { "/ct", new ChibakuTenseiCommand() },
        { "/timescale", new TimescaleCommand() },
        { "/stunclick", new StunOnClickCommand() },
        { "/start", new StartGameCommand() },
        { "/ready", new ReadyCommand() },
        { "/entrance", new EntranceCommand() }
    };

    public static void Print(string name, string? message, bool isSystem = false) {
        if (string.IsNullOrWhiteSpace(message) || !Helper.HUDManager.IsNotNull(out HUDManager hudManager)) return;
        _ = Reflector.Target(hudManager)?.InvokeInternalMethod("AddChatMessage", message, name);

        if (!isSystem && hudManager.localPlayer.isTypingChat) {
            hudManager.localPlayer.isTypingChat = false;
            hudManager.typingIndicator.enabled = false;
            hudManager.chatTextField.text = "";
            hudManager.PingHUDElement(hudManager.Chat, 1.0f, 1.0f, 0.2f);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public static void Print(string? message) {
        Console.Print("SYSTEM", message, true);
    }

    public static void ExecuteCommand(string command) {
        Console.Print("USER", command);
        Console.ExecuteCommand(command.Split(' '));
    }

    public static void ExecuteCommand(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /<command> <args>");
            return;
        }

        if (!Console.Commands.ContainsKey(args[0])) {
            Console.Print("Command not found!");
            return;
        }

        Console.Commands[args[0]].Execute(args[1..]);
    }
}



