using System.Collections.Generic;
using GameNetcodeStuff;

namespace Hax;

public class HateCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /hate <player> <funnyRevive>");
            return;
        }

        if (!Helper.GetActivePlayer(args[0]).IsNotNull(out PlayerControllerB targetPlayer)) {
            Console.Print("Player not found!");
            return;
        }

        List<string> promptedEnemies = Helper.PromptEnemiesToTarget(targetPlayer, false);

        if (promptedEnemies.Count is 0) {
            Console.Print("No enemies found!");
            return;
        }

        promptedEnemies.ForEach(enemy => Console.Print($"{enemy} prompted!"));
        Console.Print($"Enemies prompted: {promptedEnemies.Count}");
    }
}
