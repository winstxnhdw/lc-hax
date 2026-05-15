using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("hate")]
sealed class HateCommand : IEnemyPrompter, ICommand {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: hate <player> <funnyRevive>");
            return Task.CompletedTask;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Target player is not alive or found!");
            return Task.CompletedTask;
        }

        List<string> promptedEnemies = this.PromptEnemiesToTarget(targetPlayer: targetPlayer);

        if (promptedEnemies.Count is 0) {
            Chat.Print("No enemies found!");
            return Task.CompletedTask;
        }

        promptedEnemies.ForEach(enemy => Chat.Print($"{enemy} prompted!"));
        Chat.Print($"Enemies prompted: {promptedEnemies.Count}");
        return Task.CompletedTask;
    }
}
