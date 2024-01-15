using System;
using System.Collections.Generic;
using GameNetcodeStuff;
using Hax;

[Command("/mob")]
public class MobCommand : IEnemyPrompter, ICommand {
    public void Execute(ReadOnlySpan<string> args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /mob <player>");
            return;
        }

        if (!Helper.GetActivePlayer(args[0]).IsNotNull(out PlayerControllerB targetPlayer)) {
            Chat.Print("Player not found!");
            return;
        }

        List<string> mobs = this.PromptEnemiesToTarget(targetPlayer, willTeleportEnemies: true);

        if (mobs.Count is 0) {
            Chat.Print("No mobs found!");
            return;
        }

        mobs.ForEach(enemy => Chat.Print($"{enemy} is in the mob!"));
    }
}
