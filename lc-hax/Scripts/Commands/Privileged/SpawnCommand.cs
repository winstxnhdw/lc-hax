using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using ZLinq;

[PrivilegedCommand("spawn")]
sealed class SpawnCommand : ICommand {
    static bool IsHostileEnemy(EnemyType enemy) =>
        !enemy.enemyName.Contains("Docile Locust Bees", StringComparison.OrdinalIgnoreCase) ||
        !enemy.enemyName.Contains("Manticoil", StringComparison.OrdinalIgnoreCase);

    static Lazy<Dictionary<string, EnemyType>> HostileEnemies { get; } = new(() =>
        Resources.FindObjectsOfTypeAll<EnemyType>()
                 .AsValueEnumerable()
                 .Where(SpawnCommand.IsHostileEnemy)
                 .GroupBy(enemy => enemy.enemyName)
                 .ToDictionary(
                     enemyGroup => enemyGroup.Key,
                     enemy => enemy.Single()
                 )
    );

    void SpawnEnemyOnPlayer(PlayerControllerB player, EnemyType enemyType, ulong amount = 1) {
        if (Helper.RoundManager is not RoundManager { IsServer: true } roundManager) return;

        for (ulong i = 0; i < amount; i++) {
            NetworkObjectReference enemyReference = roundManager.SpawnEnemyGameObject(
                player.transform.position,
                0.0f,
                enemyNumber: -1,
                enemyType: enemyType
            );

            if (!enemyReference.TryGet(out NetworkObject networkObject)) {
                continue;
            }

            if (!networkObject.TryGetComponent(out EnemyAI enemy)) {
                continue;
            }

            if (networkObject.OwnerClientId != player.actualClientId) {
                networkObject.ChangeOwnership(player.actualClientId);
            }

            enemy.ChangeEnemyOwnerServerRpc(player.actualClientId);
        }
    }

    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length < 2) {
            Chat.Print("Usage: spawn <enemy> <player> <amount?>");
            return Task.CompletedTask;
        }

        string? enemy = args[0];

        if (string.IsNullOrWhiteSpace(enemy)) {
            Chat.Print($"Invalid {nameof(enemy)} name!");
            return Task.CompletedTask;
        }

        if (Helper.GetActivePlayer(args[1]) is not { } targetPlayer) {
            Chat.Print("Player is not alive or found!");
            return Task.CompletedTask;
        }

        if (!enemy.FuzzyMatch(SpawnCommand.HostileEnemies.Value.Keys, out string key)) {
            Chat.Print($"Invalid {nameof(enemy)} name!");
            return Task.CompletedTask;
        }

        if (!args[2].TryParse(defaultValue: 1, result: out ulong count)) {
            Chat.Print($"Enemy {nameof(count)} must be a positive number!");
            return Task.CompletedTask;
        }

        this.SpawnEnemyOnPlayer(targetPlayer, SpawnCommand.HostileEnemies.Value[key], count);
        Chat.Print($"Spawning {count}x {key} on {targetPlayer.playerUsername}!");
        return Task.CompletedTask;
    }
}
