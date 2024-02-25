using System.Collections.Generic;
using GameNetcodeStuff;
using Hax;

[Command("mob")]
class MobCommand : IEnemyPrompter, ICommand {
    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: mob <player>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        List<string> mobs = this.PromptEnemiesToTarget(targetPlayer: targetPlayer, willTeleportEnemies: true);

        if (mobs.Count is 0) {
            Chat.Print("No mobs found!");
            return;
        }

        mobs.ForEach(enemy => Chat.Print($"{enemy} is in the mob!"));
    }
}
