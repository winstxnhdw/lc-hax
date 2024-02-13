using System;
using System.Collections.Generic;
using GameNetcodeStuff;
using Hax;

[Command("fatality")]
internal class FatalityCommand : ICommand {
    string? HandleEnemy<T>(PlayerControllerB targetPlayer, Action<PlayerControllerB, T> enemyHandler) where T : EnemyAI {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer || Helper.GetEnemy<T>() is not T enemy) {
            return "Enemy has not yet spawned!";
        }

        enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
        enemy.transform.position = targetPlayer.transform.position;
        enemy.SyncPositionToClients();
        enemyHandler(targetPlayer, enemy);
        enemy.ChangeEnemyOwnerServerRpc(targetPlayer.actualClientId);
        return null;
    }

    void GiantFatality(PlayerControllerB targetPlayer, ForestGiantAI forestGiant) => forestGiant.GrabPlayerServerRpc(targetPlayer.PlayerIndex());

    void JesterFatality(PlayerControllerB targetPlayer, JesterAI jester) => jester.KillPlayerServerRpc(targetPlayer.PlayerIndex());

    void MaskedFatality(PlayerControllerB targetPlayer, MaskedPlayerEnemy masked) => masked.KillPlayerAnimationServerRpc(targetPlayer.PlayerIndex());

    void BaboonHawkFatality(PlayerControllerB targetPlayer, BaboonBirdAI baboonHawk) => baboonHawk.StabPlayerDeathAnimServerRpc(targetPlayer.PlayerIndex());

    void BeesFatality(PlayerControllerB targetPlayer, RedLocustBees bees) => bees.BeeKillPlayerServerRpc(targetPlayer.PlayerIndex());

    void EyelessDogFatality(PlayerControllerB targetPlayer, MouthDogAI eyelessDog) => eyelessDog.KillPlayerServerRpc(targetPlayer.PlayerIndex());

    void BrackenFatality(PlayerControllerB targetPlayer, FlowermanAI bracken) => bracken.KillPlayerAnimationServerRpc(targetPlayer.PlayerIndex());

    void NutcrackerFatality(PlayerControllerB targetPlayer, NutcrackerEnemyAI nutcracker) {
        nutcracker.AimGunServerRpc(targetPlayer.transform.position);
        nutcracker.FireGunServerRpc();
        nutcracker.LegKickPlayerServerRpc(targetPlayer.PlayerIndex());
    }

    public void Execute(StringArray args) {
        if (args.Length < 2) {
            Chat.Print("Usage: fatality <player> <enemy>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        Dictionary<string, Func<string?>> enemyHandlers = new() {
            { "Forest Giant", () => this.HandleEnemy<ForestGiantAI>(targetPlayer, this.GiantFatality) },
            { "Jester",       () => this.HandleEnemy<JesterAI>(targetPlayer, this.JesterFatality) },
            { "Masked",       () => this.HandleEnemy<MaskedPlayerEnemy>(targetPlayer, this.MaskedFatality) },
            { "Baboon Hawk",  () => this.HandleEnemy<BaboonBirdAI>(targetPlayer, this.BaboonHawkFatality) },
            { "Circuit Bees", () => this.HandleEnemy<RedLocustBees>(targetPlayer, this.BeesFatality) },
            { "Eyeless Dog",  () => this.HandleEnemy<MouthDogAI>(targetPlayer, this.EyelessDogFatality) },
            { "Bracken",      () => this.HandleEnemy<FlowermanAI>(targetPlayer, this.BrackenFatality) },
            { "Nutcracker",   () => this.HandleEnemy<NutcrackerEnemyAI>(targetPlayer, this.NutcrackerFatality) }
        };

        string? key = Helper.FuzzyMatch(
            string.Join(" ", args[1..]).ToTitleCase(),
            enemyHandlers.Keys
        );

        if (string.IsNullOrWhiteSpace(key)) {
            Chat.Print("Failed to find enemy!");
            return;
        }

        Chat.Print($"Performing {key} fatality on {targetPlayer.playerUsername}..");

        if (enemyHandlers[key]() is string message) {
            Chat.Print(message);
        }
    }
}
