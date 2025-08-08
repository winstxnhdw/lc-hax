using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("fatality")]
sealed class FatalityCommand : ICommand {
    /// <summary>
    /// Teleports the enemy to the target player and perform the fatality.
    /// Teleporting certain enemies outside of the factory can lag the user, so this burden is passed to the target player.
    /// </summary>
    /// <returns>true if the enemy was successfully teleported and the fatality was performed</returns>
    static bool HandleEnemy<T>(PlayerControllerB targetPlayer, Action<PlayerControllerB, T> enemyHandler) where T : EnemyAI {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return false;
        if (Helper.GetEnemy<T>() is not T enemy) return false;

        enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
        enemy.transform.position = targetPlayer.transform.position;
        enemy.SyncPositionToClients();
        enemyHandler(targetPlayer, enemy);
        enemy.ChangeEnemyOwnerServerRpc(targetPlayer.actualClientId);

        return true;
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

    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length < 2) {
            Chat.Print("Usage: fatality <player> <enemy>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        Dictionary<string, Func<bool>> enemyHandlers = new() {
            { "Forest Giant", () => FatalityCommand.HandleEnemy<ForestGiantAI>(targetPlayer, this.GiantFatality) },
            { "Jester",       () => FatalityCommand.HandleEnemy<JesterAI>(targetPlayer, this.JesterFatality) },
            { "Masked",       () => FatalityCommand.HandleEnemy<MaskedPlayerEnemy>(targetPlayer, this.MaskedFatality) },
            { "Baboon Hawk",  () => FatalityCommand.HandleEnemy<BaboonBirdAI>(targetPlayer, this.BaboonHawkFatality) },
            { "Circuit Bees", () => FatalityCommand.HandleEnemy<RedLocustBees>(targetPlayer, this.BeesFatality) },
            { "Eyeless Dog",  () => FatalityCommand.HandleEnemy<MouthDogAI>(targetPlayer, this.EyelessDogFatality) },
            { "Bracken",      () => FatalityCommand.HandleEnemy<FlowermanAI>(targetPlayer, this.BrackenFatality) },
            { "Nutcracker",   () => FatalityCommand.HandleEnemy<NutcrackerEnemyAI>(targetPlayer, this.NutcrackerFatality) }
        };

        if (!string.Join(" ", args[1..]).ToTitleCase().FuzzyMatch(enemyHandlers.Keys, out string key)) {
            Chat.Print("Failed to find enemy!");
            return;
        }

        Chat.Print($"Performing {key} fatality on {targetPlayer.playerUsername}..");

        if (!enemyHandlers[key]()) {
            Chat.Print("Enemy has not yet spawned!");
        }
    }
}
