using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using GameNetcodeStuff;

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

    void SpawnEnemyOnPlayer(PlayerControllerB player, GameObject prefab, ulong amount = 1) {
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

    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (args.Length < 2) {
            Chat.Print("Usage: spawn <enemy> <player> <amount?>");
            return;
        }

        if (args[0] is not string enemy) {
            Chat.Print("Invalid enemy!");
            return;
        }

        if (Helper.GetActivePlayer(args[1]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Player is not alive or found!");
            return;
        }

        if (!enemy.FuzzyMatch(SpawnCommand.HostileEnemies.Value.Keys, out string key)) {
            Chat.Print("Invalid enemy!");
            return;
        }

        if (!args[2].TryParse(defaultValue: 1, result: out ulong amount)) {
            Chat.Print("Invalid amount specified. Defaulting to 1.");
            return;
        }

        this.SpawnEnemyOnPlayer(targetPlayer, SpawnCommand.HostileEnemies.Value[key], amount);
        Chat.Print($"Spawning {amount}x {key} on {targetPlayer.playerUsername}.");
    }
}
