using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;
using ZLinq;

[Command("bombard")]
sealed class BombardCommand : ICommand, IJetpack {
    const float BombardRadius = 10.0f;

    async Task ExplodeJetpackAsync(
        JetpackItem jetpack,
        Transform targetTransform,
        CancellationToken cancellationToken
    ) {
        await Helper.WaitUntil(
            () =>
                jetpack == null || Vector3.Distance(jetpack.transform.position, targetTransform.position) < 5.0f,
            cancellationToken
        );

        float explosionTime = Time.time + 0.5f;
        await Helper.WaitUntil(() => jetpack == null || Time.time >= explosionTime, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        jetpack.Unfake()?.ExplodeJetpackServerRpc();
    }

    /// <summary>
    /// Grab and discard jetpacks to a random location of the same elevation near the target player.
    /// If the target player is far away, it may take a while for the jetpacks to reach the player.
    /// The jetpacks will only explode if they within 5 units of the target player.
    /// </summary>
    async Task BombardAsync(
        PlayerControllerB player,
        Transform targetTransform,
        JetpackItem[] jetpacks,
        CancellationToken cancellationToken
    ) {
        List<Task> explosions = [];
        float currentWeight = player.carryWeight;

        foreach (JetpackItem jetpack in jetpacks) {
            if (jetpack == null) continue;
            if (!player.GrabObject(jetpack)) continue;

            int grabFrame = Time.frameCount;

            await Helper.WaitUntil(() =>
                Time.frameCount != grabFrame && (jetpack == null || player.currentlyHeldObjectServer == jetpack),
                cancellationToken
            );

            if (jetpack == null) {
                continue;
            }

            Vector2 randomDirection = Random.insideUnitCircle * BombardCommand.BombardRadius;
            Vector3 randomDirectionXZ = new(randomDirection.x, 0.0f, randomDirection.y);

            cancellationToken.ThrowIfCancellationRequested();
            player.DiscardHeldObject(placeObject: true, placePosition: targetTransform.position + randomDirectionXZ);
            explosions.Add(this.ExplodeJetpackAsync(jetpack, targetTransform, cancellationToken));
            await Task.Delay(100, cancellationToken);
        }

        player.carryWeight = currentWeight;
        await Task.WhenAll(explosions);
    }

    async Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not { } localPlayer) return;
        if (args.Length is 0) {
            Chat.Print("Usage: bombard <player>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not { } targetPlayer) {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        JetpackItem[] jetpacks = this.GetAvailableJetpacks();

        if (jetpacks.Length is 0) {
            Chat.Print("A usable jetpack is required to use this command!");
            return;
        }

        if (!jetpacks.AsValueEnumerable().Any(jetpack => localPlayer.FirstEmptyItemSlot(jetpack) != -1)) {
            Chat.Print("You must have an empty inventory slot!");
            return;
        }

        await this.BombardAsync(localPlayer, targetPlayer.transform, jetpacks, cancellationToken);
    }
}
