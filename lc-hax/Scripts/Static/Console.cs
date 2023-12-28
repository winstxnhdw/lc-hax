using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Hax;

public static class Console {
    static Dictionary<string, ICommand> Commands { get; } = new() {
        { "/god", new GodCommand() },
        { "/end", new EndCommand() },
        { "/mob", new MobCommand() },
        { "/hate", new HateCommand() },
        { "/kill", new KillCommand() },
        { "/lock", new LockCommand() },
        { "/home", new HomeCommand() },
        { "/stun", new StunCommand() },
        { "/heal", new HealCommand() },
        { "/beta", new BetaCommand() },
        { "/grab", new GrabCommand() },
        { "/block", new BlockCommand() },
        { "/visit", new VisitCommand() },
        { "/build", new BuildCommand() },
        { "/tp", new TeleportCommand() },
        { "/noise", new NoiseCommand() },
        { "/money", new MoneyCommand() },
        { "/xyz", new LocationCommand() },
        { "/xp", new ExperienceCommand() },
        { "/shovel", new ShovelCommand() },
        { "/unlock", new UnlockCommand() },
        { "/random", new RandomCommand() },
        { "/demigod", new DemiGodCommand() },
        { "/start", new StartGameCommand() },
        { "/pumpkin", new PumpkinCommand() },
        { "/players", new PlayersCommand() },
        { "/explode", new ExplodeCommand() },
        { "/ct", new ChibakuTenseiCommand() },
        { "/entrance", new EntranceCommand() },
        { "/enter", new EntranceCommand(inside: true) },
        { "/exit", new EntranceCommand() },
        { "/stunclick", new StunOnClickCommand() },
        { "/levels", new Debug(new LevelsCommand()) },
        { "/timescale", new Debug(new TimescaleCommand()) },
        { "/fixcamera", new Debug(new FixCameraCommand()) },
        { "/unlockables", new Debug(new UnlockablesCommand()) },
    };

    public static void Print(string name, string? message, bool isSystem = false) {
        if (string.IsNullOrWhiteSpace(message) || !Helper.HUDManager.IsNotNull(out HUDManager hudManager)) return;
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



