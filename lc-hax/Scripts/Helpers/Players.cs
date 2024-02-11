using System;
using System.Linq;
using System.Runtime.CompilerServices;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace Hax;

internal static partial class Helper {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int PlayerIndex(this PlayerControllerB player) => unchecked((int)player.playerClientId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsSelf(this PlayerControllerB? instance) => Helper.LocalPlayer is PlayerControllerB localPlayer && instance?.actualClientId == localPlayer.actualClientId;

    internal static void DamagePlayerRpc(this PlayerControllerB player, int damage) =>
        player.DamagePlayerFromOtherClientServerRpc(damage, Vector3.zero, -1);

    internal static void HealPlayer(this PlayerControllerB player) => player.DamagePlayerRpc(-100);

    internal static void KillPlayer(this PlayerControllerB player) => player.DamagePlayerRpc(100);

    internal static void EntranceTeleport(this PlayerControllerB player, bool outside) {
        player.TeleportPlayer(RoundManager.FindMainEntranceScript(outside).entrancePoint.position);
        player.isInsideFactory = !outside;
    }

    internal static PlayerControllerB? LocalPlayer => Helper.GameNetworkManager?.localPlayerController.Unfake();

    internal static PlayerControllerB[] Players => Helper.StartOfRound?.allPlayerScripts ?? [];

    internal static PlayerControllerB[] ActivePlayers => Helper.Players.Where(player => player.isPlayerControlled && !player.isPlayerDead).ToArray();

    internal static PlayerControllerB? GetPlayer(string? playerNameOrId) {
        if (string.IsNullOrEmpty(playerNameOrId)) return null;

        PlayerControllerB[] players = Helper.Players;

        return players.First(player => player.playerUsername.ToLower().Contains(playerNameOrId.ToLower(), StringComparison.InvariantCultureIgnoreCase)) ??
               players.First(player => player.playerClientId.ToString() == playerNameOrId);
    }

    internal static PlayerControllerB? GetPlayer(ulong playerClientId) => Helper.Players.First(player => player.playerClientId == playerClientId);

    internal static PlayerControllerB? GetPlayer(int playerClientId) => Helper.GetPlayer(unchecked((ulong)playerClientId));

    internal static PlayerControllerB? GetActivePlayer(string? playerNameOrId) {
        PlayerControllerB? player = Helper.GetPlayer(playerNameOrId);
        return player == null || player.IsDead() ? null : player;
    }

    internal static PlayerControllerB? GetActivePlayer(int playerClientId) => Helper.GetActivePlayer(playerClientId.ToString());

    internal static bool GrabObject(this PlayerControllerB player, GrabbableObject grabbable) {
        if (player.ItemSlots.WhereIsNotNull().Count() >= 4) return false;

        NetworkObjectReference networkObject = grabbable.NetworkObject;
        _ = player.Reflect().InvokeInternalMethod("GrabObjectServerRpc", networkObject);

        grabbable.parentObject = player.localItemHolder;
        grabbable.GrabItemOnClient();

        return true;
    }

    internal static bool IsDead(this PlayerControllerB? instance) => instance is null ? false : !instance.isPlayerControlled || instance.health <= 0;

    internal static PlayerControllerB? GetPlayerFromBody(this RagdollGrabbableObject body) => Helper.GetPlayer(body.bodyID.Value);

}


