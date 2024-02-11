using System;
using System.Linq;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using GameNetcodeStuff;
using Hax;

[PrivilegedCommand("spawn")]
internal class SpawnCommand : ICommand {
    static bool IsHostileEnemy(EnemyType enemy) =>
        !enemy.enemyName.Contains("Docile Locust Bees", StringComparison.OrdinalIgnoreCase) ||
        !enemy.enemyName.Contains("Manticoil", StringComparison.OrdinalIgnoreCase);

    static Dictionary<string, GameObject> HostileEnemies { get; } =
        Resources.FindObjectsOfTypeAll<EnemyType>()
                 .Where(SpawnCommand.IsHostileEnemy)
                 .GroupBy(enemy => enemy.enemyName)
                 .ToDictionary(enemyGroup => enemyGroup.Key, enemy => Enumerable.First(enemy).enemyPrefab);

    void SpawnEnemyOnPlayer(PlayerControllerB player, GameObject prefab, ulong amount = 1) {
        for (ulong i = 0; i < amount; i++) {
            GameObject enemy = UnityObject.Instantiate(prefab, player.transform.position, Quaternion.identity);

            if (!enemy.TryGetComponent(out NetworkObject networkObject)) {
                UnityObject.Destroy(enemy);
                continue;
            }

            networkObject.Spawn(true);
        }
    }

    public void Execute(StringArray args) {
        if (Helper.RoundManager?.currentLevel is null) return;
        if (args.Length < 2) {
            Chat.Print("Usage: spawn <enemy> <player> <amount?>");
            return;
        }

        if (Helper.GetActivePlayer(args[1]) is not PlayerControllerB targetPlayer) {
            Chat.Print($"{args[1]} is not alive or found!");
            return;
        }

        string? key = Helper.FuzzyMatch(args[0], SpawnCommand.HostileEnemies.Keys);

        if (key is null) {
            Chat.Print("Invalid enemy!");
            return;
        }

        if (!args[2].TryParse(defaultValue: 1, result: out ulong amount)) {
            Chat.Print("Invalid amount specified. Defaulting to 1.");
        }

        this.SpawnEnemyOnPlayer(targetPlayer, SpawnCommand.HostileEnemies[key], amount);
        Helper.SendNotification("Spawner", $"Spawning {(amount > 1 ? amount.ToString() : "a")} {(amount > 1 ? key + "s" : key)} on {targetPlayer.playerUsername}.", false);
    }
}
