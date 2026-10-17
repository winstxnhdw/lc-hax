using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;

[Command("fatality")]
sealed class FatalityCommand : ICommand {
    /// <summary>
    /// Performs the enemy's fatality on the target player.
    /// </summary>
    /// <returns>The result of starting the enemy's fatality.</returns>
    static async Task<Result> HandleEnemy<T>(PlayerControllerB targetPlayer, Action<PlayerControllerB, T> enemyHandler, CancellationToken cancellationToken) where T : EnemyAI {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return new Result { Message = "Local player is not available!" };
        if (targetPlayer == null || !targetPlayer.isPlayerControlled || targetPlayer.isPlayerDead) return new Result { Message = "Target player is not alive or found!" };
        if (Helper.GetEnemy<T>() is not T enemy) return new Result { Message = "Enemy has not yet spawned!" };

        enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);

        await Helper.WaitUntil(() =>
            enemy == null || !enemy.IsSpawned || targetPlayer == null || !targetPlayer.isPlayerControlled || targetPlayer.isPlayerDead ||
            enemy.IsOwner,
            cancellationToken
        );

        if (enemy == null || !enemy.IsSpawned) {
            return new Result { Message = "Enemy is no longer available!" };
        }

        if (targetPlayer == null || !targetPlayer.isPlayerControlled || targetPlayer.isPlayerDead) {
            return new Result { Message = "Target player is not alive or found!" };
        }

        cancellationToken.ThrowIfCancellationRequested();
        enemyHandler(targetPlayer, enemy);
        return new Result { Success = true };
    }

    void GiantFatality(PlayerControllerB targetPlayer, ForestGiantAI forestGiant) => forestGiant.GrabPlayerServerRpc(targetPlayer.PlayerIndex());

    void JesterFatality(PlayerControllerB targetPlayer, JesterAI jester) => jester.KillPlayerServerRpc(targetPlayer.PlayerIndex());

    void MaskedFatality(PlayerControllerB targetPlayer, MaskedPlayerEnemy masked) => masked.KillPlayerAnimationServerRpc(targetPlayer.PlayerIndex());

    void BeesFatality(PlayerControllerB targetPlayer, RedLocustBees bees) => bees.BeeKillPlayerServerRpc(targetPlayer.PlayerIndex());

    void EyelessDogFatality(PlayerControllerB targetPlayer, MouthDogAI eyelessDog) => eyelessDog.KillPlayerServerRpc(targetPlayer.PlayerIndex());

    void BrackenFatality(PlayerControllerB targetPlayer, FlowermanAI bracken) => bracken.KillPlayerAnimationServerRpc(targetPlayer.PlayerIndex());

    void ManeaterFatality(PlayerControllerB targetPlayer, CaveDwellerAI maneater) => maneater.KillPlayerAnimationServerRpc(targetPlayer.PlayerIndex());

    void NutcrackerFatality(PlayerControllerB targetPlayer, NutcrackerEnemyAI nutcracker) {
        nutcracker.SwitchTargetServerRpc(targetPlayer.PlayerIndex());

        if (nutcracker.gun is ShotgunItem gun) {
            Vector3 position = gun.shotgunRayPoint.position;
            Vector3 direction = (targetPlayer.playerCollider.bounds.center - position).normalized;
            gun.ShootGunServerRpc(position, direction);
        }

        nutcracker.LegKickPlayerServerRpc(targetPlayer.PlayerIndex());
    }

    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length < 2) {
            Chat.Print("Usage: fatality <player> <enemy>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        Dictionary<string, Func<Task<Result>>> enemyHandlers = new() {
            { "Forest Giant", () => FatalityCommand.HandleEnemy<ForestGiantAI>(targetPlayer, this.GiantFatality, cancellationToken) },
            { "Jester",       () => FatalityCommand.HandleEnemy<JesterAI>(targetPlayer, this.JesterFatality, cancellationToken) },
            { "Masked",       () => FatalityCommand.HandleEnemy<MaskedPlayerEnemy>(targetPlayer, this.MaskedFatality, cancellationToken) },
            { "Circuit Bees", () => FatalityCommand.HandleEnemy<RedLocustBees>(targetPlayer, this.BeesFatality, cancellationToken) },
            { "Eyeless Dog",  () => FatalityCommand.HandleEnemy<MouthDogAI>(targetPlayer, this.EyelessDogFatality, cancellationToken) },
            { "Bracken",      () => FatalityCommand.HandleEnemy<FlowermanAI>(targetPlayer, this.BrackenFatality, cancellationToken) },
            { "Maneater",     () => FatalityCommand.HandleEnemy<CaveDwellerAI>(targetPlayer, this.ManeaterFatality, cancellationToken) },
            { "Nutcracker",   () => FatalityCommand.HandleEnemy<NutcrackerEnemyAI>(targetPlayer, this.NutcrackerFatality, cancellationToken) }
        };

        if (!string.Join(" ", args[1..]).ToTitleCase().FuzzyMatch(enemyHandlers.Keys, out string key)) {
            Chat.Print("Failed to find enemy!");
            return;
        }

        Result result = await enemyHandlers[key]();
        Chat.Print($"Performing {key} fatality on {targetPlayer.playerUsername}..");
        Chat.Print(result.Message);
    }
}
