using System.Collections.Generic;
using GameNetcodeStuff;
using Hax;

[Command("hate")]
class HateCommand : IEnemyPrompter, ICommand {
    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: hate <player> <funnyRevive>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        List<string> promptedEnemies = this.PromptEnemiesToTarget(targetPlayer: targetPlayer);

        if (promptedEnemies.Count is 0) {
            Chat.Print("No enemies found!");
            return;
        }

        promptedEnemies.ForEach(enemy => Chat.Print($"{enemy} prompted!"));
        Chat.Print($"Enemies prompted: {promptedEnemies.Count}");
    }
}
