#region

using GameNetcodeStuff;
using Hax;
using UnityEngine;

#endregion

[HostCommand("spawn")]
class SpawnCommand : ICommand {
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

            if (args[0] is not string enemyname) {
                Chat.Print("Invalid enemy!");
                return;
            }

            if (!enemyname.FuzzyMatch(Helper.HostileEnemies.Keys, out string? key)) {
                Chat.Print("Invalid enemy!");
                return;
            }

            if (key is null) {
                Chat.Print("Invalid enemy!");
                return;
            }

            if (!args[2].TryParse(1, out ulong amount)) Chat.Print("Invalid amount specified. Defaulting to 1.");

            Helper.SpawnEnemies(targetPlayer.transform.position, Helper.HostileEnemies[key], amount);
            Helper.SendFlatNotification(
                $"Spawning {(amount > 1 ? amount.ToString() : "a")} {(amount > 1 ? key + "s" : key)} on {targetPlayer.playerUsername}.");
        }

        else {
            if (Helper.CurrentCamera is not Camera camera || !camera.enabled) return;

            if (args.Length < 1) {
                Chat.Print("Usage: spawn <enemy>");
                return;
            }

            if (args[0] is not string enemyname) {
                Chat.Print("Invalid enemy!");
                return;
            }

            if (!enemyname.FuzzyMatch(Helper.HostileEnemies.Keys, out string key)) {
                Chat.Print("Invalid enemy!");
                return;
            }

            EnemyAI? enemy = Helper.SpawnEnemy(camera.transform.position, Helper.HostileEnemies[key]);

            if (enemy is not null) {
                PossessionMod.Instance?.Possess(enemy);
                Helper.SendFlatNotification($"Spawning {key} and possessing it.");
            }
        }
    }
}
