using System;
using System.Linq;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using GameNetcodeStuff;
using Hax;

[PrivilegedCommand("spawn")]
class SpawnCommand : ICommand {
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
        if (args.Length < 2) {
            Chat.Print("Usage: spawn <enemy> <player> <amount?>");
            return;
        }

        if (Helper.GetActivePlayer(args[1]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Player is not alive or found!");
            return;
        }

        string? key = Helper.FuzzyMatch(args[0], SpawnCommand.HostileEnemies.Keys);

        if (string.IsNullOrWhiteSpace(key)) {
            Chat.Print("Invalid enemy!");
            return;
        }

        if (!args[2].TryParse(defaultValue: 1, result: out ulong amount)) {
            Chat.Print("Invalid amount specified. Defaulting to 1.");
            return;
        }

        this.SpawnEnemyOnPlayer(targetPlayer, SpawnCommand.HostileEnemies[key], amount);
        Chat.Print($"Spawning {amount}x {key} on {targetPlayer.playerUsername}.");
    }
}
