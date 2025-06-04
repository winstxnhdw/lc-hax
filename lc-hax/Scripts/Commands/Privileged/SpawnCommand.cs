using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using UnityObject = UnityEngine.Object;

[PrivilegedCommand("spawn")]
class SpawnCommand : ICommand {
    static bool IsHostileEnemy(EnemyType enemy) =>
        !enemy.enemyName.Contains("Docile Locust Bees", StringComparison.OrdinalIgnoreCase) ||
        !enemy.enemyName.Contains("Manticoil", StringComparison.OrdinalIgnoreCase);

    static Lazy<Dictionary<string, GameObject>> HostileEnemies { get; } = new(() =>
        Resources.FindObjectsOfTypeAll<EnemyType>()
                 .Where(SpawnCommand.IsHostileEnemy)
                 .GroupBy(enemy => enemy.enemyName)
                 .ToDictionary(enemyGroup => enemyGroup.Key, enemy => Enumerable.First(enemy).enemyPrefab)
    );

    static void SpawnEnemyOnPlayer(PlayerControllerB player, GameObject prefab, ulong amount = 1) {
        for (ulong i = 0; i < amount; i++) {
            GameObject enemyObject = UnityObject.Instantiate(prefab, player.transform.position, Quaternion.identity);

            if (!enemyObject.TryGetComponent(out NetworkObject networkObject)) {
                UnityObject.Destroy(enemyObject);
                continue;
            }

            if (!networkObject.TryGetComponent(out EnemyAI enemy)) {
                UnityObject.Destroy(enemyObject);
                continue;
            }

            enemy.ChangeEnemyOwnerServerRpc(player.actualClientId);
            networkObject.Spawn(true);
        }
    }

    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length < 2) {
            Chat.Print("Usage: spawn <enemy> <player> <amount?>");
            return;
        }

        string? enemy = args[0];

        if (string.IsNullOrWhiteSpace(enemy)) {
            Chat.Print($"Invalid {nameof(enemy)} name!");
            return;
        }

        if (Helper.GetActivePlayer(args[1]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Player is not alive or found!");
            return;
        }

        if (!enemy.FuzzyMatch(SpawnCommand.HostileEnemies.Value.Keys, out string key)) {
            Chat.Print($"Invalid {nameof(enemy)} name!");
            return;
        }

        if (!args[2].TryParse(defaultValue: 1, result: out ulong count)) {
            Chat.Print($"Enemy {nameof(count)} must be a positive number!");
            return;
        }

        SpawnCommand.SpawnEnemyOnPlayer(targetPlayer, SpawnCommand.HostileEnemies.Value[key], count);
        Chat.Print($"Spawning {count}x {key} on {targetPlayer.playerUsername}!");
    }
}
