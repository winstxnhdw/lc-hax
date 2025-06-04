using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("mob")]
class MobCommand : IEnemyPrompter, ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
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
