using System.Collections.Generic;
using GameNetcodeStuff;

namespace Hax;

[Command("/mob")]
public class MobCommand : IEnemyPrompter, ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /mob <player>");
            return;
        }

        if (!Helper.GetActivePlayer(args[0]).IsNotNull(out PlayerControllerB targetPlayer)) {
            Console.Print("Player not found!");
            return;
        }

        List<string> mobs = this.PromptEnemiesToTarget(targetPlayer, willTeleportEnemies: true);

        if (mobs.Count is 0) {
            Console.Print("No mobs found!");
            return;
        }

        mobs.ForEach(enemy => Console.Print($"{enemy} is in the mob!"));
    }
}
