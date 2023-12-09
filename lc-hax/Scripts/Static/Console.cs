using System.Linq;
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
        { "/home", new HomeCommand() },
        { "/end", new EndCommand() },
        { "/explode", new ExplodeCommand() },
        { "/noise", new NoiseCommand() },
        { "/random", new RandomCommand() },
        { "/stun", new StunCommand() },
        { "/heal", new HealCommand() },
        { "/ct", new ChibakuTenseiCommand() },
        { "/pumpkin", new PumpkinCommand() }
    };

    static Reflector? HUDManagerReflector => Helper.HUDManager == null ? null : Reflector.Target(Helper.HUDManager);

    public static void Print(string name, string? message) {
        if (Helper.HUDManager == null || string.IsNullOrWhiteSpace(message)) return;

        _ = Console.HUDManagerReflector?.InvokeInternalMethod("AddChatMessage", message, name);

        if (Helper.HUDManager.localPlayer.isTypingChat) {
            Helper.HUDManager.localPlayer.isTypingChat = false;
            Helper.HUDManager.typingIndicator.enabled = false;
            Helper.HUDManager.chatTextField.text = "";
            Helper.HUDManager.PingHUDElement(Helper.HUDManager.Chat, 1.0f, 1.0f, 0.2f);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public static void ExecuteCommand(string command) {
        Console.Print("USER", command);
        Console.ExecuteCommand(command.Split(' '));
    }

    public static void ExecuteCommand(string[] args) {
        if (args.Length < 1) {
            Console.Print("SYSTEM", "Usage: /<command> <args>");
            return;
        }

        if (!Console.Commands.ContainsKey(args[0])) {
            Console.Print("SYSTEM", "Command not found!");
            return;
        }

        Console.Commands[args[0]].Execute(args.Skip(1).ToArray());
    }
}



