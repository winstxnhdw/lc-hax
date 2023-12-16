using System.Collections.Generic;
using GameNetcodeStuff;

namespace Hax;

public class HateCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /hate <player> <funnyRevive>");
            return;
        }

        if (!Helper.GetPlayer(args[0]).IsNotNull(out PlayerControllerB targetPlayer)) {
            Console.Print("Player not found!");
            return;
        }

        bool funnyRevive = false;
        if (args.Length is 2 &&
            !bool.TryParse(args[1], out funnyRevive)) {
            Console.Print("funnyRevive parse failed, set to false!");
            return;
        }

        List<string> promptedEnemies = Helper.PromptEnemiesToTarget(targetPlayer, funnyRevive);

        if (promptedEnemies.Count is 0) {
            Console.Print("No enemies found!");
            return;
        }

        promptedEnemies.ForEach(enemy => Console.Print($"{enemy} prompted!"));
        Console.Print($"Enemies prompted: {promptedEnemies.Count}");
    }
}
