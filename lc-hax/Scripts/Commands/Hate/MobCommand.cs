using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("mob")]
sealed class MobCommand : IEnemyPrompter, ICommand {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: mob <player>");
            return Task.CompletedTask;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Target player is not alive or found!");
            return Task.CompletedTask;
        }

        List<string> mobs = this.PromptEnemiesToTarget(targetPlayer: targetPlayer, willTeleportEnemies: true);

        if (mobs.Count is 0) {
            Chat.Print("No mobs found!");
            return Task.CompletedTask;
        }

        mobs.ForEach(enemy => Chat.Print($"{enemy} is in the mob!"));
        return Task.CompletedTask;
    }
}
