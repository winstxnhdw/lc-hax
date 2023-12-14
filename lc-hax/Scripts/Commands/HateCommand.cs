using System.Collections.Generic;
using GameNetcodeStuff;

namespace Hax;

public class HateCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Helper.PrintSystem("Usage: /hate <player>");
            return;
        }

        if (!Helper.GetPlayer(args[0]).IsNotNull(out PlayerControllerB targetPlayer)) {
            Helper.PrintSystem("Player not found!");
            return;
        }

        List<string> promptedEnemies = Helper.PromptEnemiesToTarget(targetPlayer);

        if (promptedEnemies.Count is 0) {
            Helper.PrintSystem("No enemies found!");
            return;
        }

        promptedEnemies.ForEach(enemy => Helper.PrintSystem($"{enemy} prompted!"));
        Helper.PrintSystem($"Enemies prompted: {promptedEnemies.Count}");
    }
}
