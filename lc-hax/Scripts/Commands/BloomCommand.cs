using System;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;

[Command("bloom")]
sealed class BloomCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: bloom <target>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        if (Helper.GetEnemy<CadaverGrowthAI>() is not CadaverGrowthAI growth) {
            Chat.Print("Cadaver growth has not yet spawned!");
            return;
        }

        if (!Array.Exists(growth.bloomEnemies, bloom => bloom == null)) {
            Chat.Print("Bloom pool is full!");
            return;
        }

        int targetId = targetPlayer.PlayerIndex();
        cancellationToken.ThrowIfCancellationRequested();
        growth.CurePlayer(targetId);
        growth.CurePlayerRpc(targetId);
        growth.InfectPlayer(targetPlayer, severe: false);
        growth.InfectPlayerRpc(targetId, severe: false);

        await Helper.WaitUntil(() =>
            growth == null || !growth.IsSpawned || targetPlayer == null || !targetPlayer.isPlayerControlled ||
            Array.Exists(growth.bloomEnemies, bloom => bloom != null && bloom.IsSpawned && !bloom.hasBurst),
            cancellationToken
        );

        if (growth == null || !growth.IsSpawned || targetPlayer == null || !targetPlayer.isPlayerControlled) {
            return;
        }

        Vector3 position = targetPlayer.transform.position;
        Vector3 rotation = targetPlayer.transform.eulerAngles;
        cancellationToken.ThrowIfCancellationRequested();
        growth.BurstFromPlayer(targetPlayer, position, rotation);
        growth.SyncBurstFromPlayerRpc(targetId, position, rotation);
        Chat.Print($"Blooming {targetPlayer.playerUsername}..");
    }
}
