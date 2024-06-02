using System;
using System.Collections.Generic;
using GameNetcodeStuff;
using Hax;

[Command("fatality")]
internal class FatalityCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (args.Length < 2)
        {
            Chat.Print("Usage: fatality <player> <enemy>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer)
        {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        Dictionary<string, Func<bool>> enemyHandlers = new()
        {
            { "Forest Giant", () => HandleEnemy<ForestGiantAI>(targetPlayer, GiantFatality) },
            { "Jester", () => HandleEnemy<JesterAI>(targetPlayer, JesterFatality) },
            { "Masked", () => HandleEnemy<MaskedPlayerEnemy>(targetPlayer, MaskedFatality) },
            { "Baboon Hawk", () => HandleEnemy<BaboonBirdAI>(targetPlayer, BaboonHawkFatality) },
            { "Circuit Bees", () => HandleEnemy<RedLocustBees>(targetPlayer, BeesFatality) },
            { "Eyeless Dog", () => HandleEnemy<MouthDogAI>(targetPlayer, EyelessDogFatality) },
            { "Bracken", () => HandleEnemy<FlowermanAI>(targetPlayer, BrackenFatality) },
            { "Nutcracker", () => HandleEnemy<NutcrackerEnemyAI>(targetPlayer, NutcrackerFatality) }
        };

        if (!string.Join(" ", args[1..]).ToTitleCase().FuzzyMatch(enemyHandlers.Keys, out var key))
        {
            Chat.Print("Failed to find enemy!");
            return;
        }

        Chat.Print($"Performing {key} fatality on {targetPlayer.playerUsername}..");

        if (!enemyHandlers[key]()) Chat.Print("Enemy has not yet spawned!");
    }

    /// <summary>
    ///     Teleports the enemy to the target player and perform the fatality.
    ///     Teleporting certain enemies outside of the factory can lag the user, so this burden is passed to the target player.
    /// </summary>
    /// <returns>true if the enemy was successfully teleported and the fatality was performed</returns>
    private bool HandleEnemy<T>(PlayerControllerB targetPlayer, Action<PlayerControllerB, T> enemyHandler)
        where T : EnemyAI
    {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer || Helper.GetEnemy<T>() is not T enemy)
            return false;

        enemy.TakeOwnership();
        enemy.transform.position = targetPlayer.transform.position;
        enemy.SyncPositionToClients();
        enemyHandler(targetPlayer, enemy);
        enemy.SetOwner(targetPlayer);

        return true;
    }

    private void GiantFatality(PlayerControllerB targetPlayer, ForestGiantAI forestGiant)
    {
        forestGiant.GrabPlayerServerRpc(targetPlayer.GetPlayerID());
    }

    private void JesterFatality(PlayerControllerB targetPlayer, JesterAI jester)
    {
        jester.KillPlayerServerRpc(targetPlayer.GetPlayerID());
    }

    private void MaskedFatality(PlayerControllerB targetPlayer, MaskedPlayerEnemy masked)
    {
        masked.KillPlayerAnimationServerRpc(targetPlayer.GetPlayerID());
    }

    private void BaboonHawkFatality(PlayerControllerB targetPlayer, BaboonBirdAI baboonHawk)
    {
        baboonHawk.StabPlayerDeathAnimServerRpc(targetPlayer.GetPlayerID());
    }

    private void BeesFatality(PlayerControllerB targetPlayer, RedLocustBees bees)
    {
        bees.BeeKillPlayerServerRpc(targetPlayer.GetPlayerID());
    }

    private void EyelessDogFatality(PlayerControllerB targetPlayer, MouthDogAI eyelessDog)
    {
        eyelessDog.KillPlayerServerRpc(targetPlayer.GetPlayerID());
    }

    private void BrackenFatality(PlayerControllerB targetPlayer, FlowermanAI bracken)
    {
        bracken.KillPlayerAnimationServerRpc(targetPlayer.GetPlayerID());
    }

    private void NutcrackerFatality(PlayerControllerB targetPlayer, NutcrackerEnemyAI nutcracker)
    {
        nutcracker.AimGunServerRpc(targetPlayer.transform.position);
        nutcracker.FireGunServerRpc();
        nutcracker.LegKickPlayerServerRpc(targetPlayer.GetPlayerID());
    }
}