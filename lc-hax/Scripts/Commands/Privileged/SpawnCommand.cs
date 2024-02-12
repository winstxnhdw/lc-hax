using GameNetcodeStuff;
using Hax;
using UnityEngine;

[PrivilegedCommand("spawn")]
internal class SpawnCommand : ICommand {
    public void Execute(StringArray args) {
        if (Helper.RoundManager?.currentLevel is null) return;

        if (!Setting.EnablePhantom) {
            if (args.Length < 2) {
                Chat.Print("Usage: spawn <enemy> <player> <amount?>");
                return;
            }

            if (Helper.GetActivePlayer(args[1]) is not PlayerControllerB targetPlayer) {
                Chat.Print($"{args[1]} is not alive or found!");
                return;
            }

            string? key = Helper.FuzzyMatch(args[0], Helper.HostileEnemies.Keys);

            if (key is null) {
                Chat.Print("Invalid enemy!");
                return;
            }

            if (!args[2].TryParse(defaultValue: 1, result: out ulong amount)) {
                Chat.Print("Invalid amount specified. Defaulting to 1.");
            }

            Helper.SpawnEnemies(targetPlayer.transform.position, Helper.HostileEnemies[key], amount);
            Helper.SendNotification("Spawner",
                $"Spawning {(amount > 1 ? amount.ToString() : "a")} {(amount > 1 ? key + "s" : key)} on {targetPlayer.playerUsername}.",
                false);
        }

        else {
            if (Helper.CurrentCamera is not Camera camera || !camera.enabled) return;

            if (args.Length < 1) {
                Chat.Print("Usage: spawn <enemy>");
                return;
            }

            string? key = Helper.FuzzyMatch(args[0], Helper.HostileEnemies.Keys);
            if (key is null) {
                Chat.Print("Invalid enemy!");
                return;
            }

            EnemyAI? enemy = Helper.SpawnEnemy(camera.transform.position, Helper.HostileEnemies[key]);

            if (enemy is not null) {
                PossessionMod.Instance?.Possess(enemy);
                Helper.SendNotification("Spawner", $"Spawning {key} and possessing it.", false);
            }
        }
    }
}
