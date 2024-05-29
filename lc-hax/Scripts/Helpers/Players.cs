#pragma warning disable CS8625

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace Hax;

internal static partial class Helper
{
    /// <summary>
    ///     Local Player
    /// </summary>
    internal static PlayerControllerB? LocalPlayer => GameNetworkManager?.localPlayerController.Unfake();


    /// <summary>
    ///     Host Player
    /// </summary>
    internal static PlayerControllerB? HostPlayer => Players[0];


    /// <summary>
    ///     All Players (including non initialized players & Dead)
    /// </summary>
    internal static PlayerControllerB[] Players => StartOfRound?.allPlayerScripts ?? [];

    /// <summary>
    ///     Active Players (Players that are not dead)
    /// </summary>
    internal static PlayerControllerB[] ActivePlayers =>
        Players.Where(player => player.isPlayerControlled && !player.isPlayerDead).ToArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int PlayerIndex(this PlayerControllerB player)
    {
        return unchecked((int)player.playerClientId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsSelf(this PlayerControllerB? player)
    {
        return LocalPlayer is PlayerControllerB localPlayer && player?.actualClientId == localPlayer.actualClientId;
    }

    internal static void DamagePlayerRpc(this PlayerControllerB player, int damage)
    {
        player.DamagePlayerFromOtherClientServerRpc(damage, Vector3.zero, -1);
    }

    internal static void HealPlayer(this PlayerControllerB player)
    {
        player.DamagePlayerRpc(-100);
    }

    internal static void KillPlayer(this PlayerControllerB player)
    {
        player.DamagePlayerRpc(100);
    }

    internal static void EntranceTeleport(this PlayerControllerB player, bool outside)
    {
        player.TeleportPlayer(RoundManager.FindMainEntranceScript(outside).entrancePoint.position);
        player.isInsideFactory = !outside;
    }

    /// <summary>
    ///     Get a player by their name or ID
    /// </summary>
    /// <param name="playerNameOrId"></param>
    /// <returns></returns>
    internal static PlayerControllerB? GetPlayer(string? playerNameOrId)
    {
        if (string.IsNullOrEmpty(playerNameOrId)) return null;

        var players = Players;

        return players.First(player =>
                   player.playerUsername.ToLower().Contains(playerNameOrId.ToLower(),
                       StringComparison.InvariantCultureIgnoreCase)) ??
               players.First(player => player.playerClientId.ToString() == playerNameOrId);
    }

    /// <summary>
    ///     Get a player by their Client ID
    /// </summary>
    /// <param name="playerClientId"></param>
    /// <returns></returns>
    internal static PlayerControllerB? GetPlayer(ulong playerClientId)
    {
        return Players.First(player => player.playerClientId == playerClientId);
    }

    /// <summary>
    ///     Get a player by their Client ID
    /// </summary>
    /// <param name="playerClientId"></param>
    /// <returns></returns>
    internal static PlayerControllerB? GetPlayer(int playerClientId)
    {
        return GetPlayer(unchecked((ulong)playerClientId));
    }

    /// <summary>
    ///     Get a player by their Client ID and is not dead
    /// </summary>
    /// <param name="playerNameOrId"></param>
    /// <returns></returns>
    internal static PlayerControllerB? GetActivePlayer(string? playerNameOrId)
    {
        var player = GetPlayer(playerNameOrId);
        return player == null || player.IsDead() ? null : player;
    }

    /// <summary>
    ///     Get a player by their Client ID and is not dead
    /// </summary>
    /// <param name="playerClientId"></param>
    /// <returns></returns>
    internal static PlayerControllerB? GetActivePlayer(int playerClientId)
    {
        return GetActivePlayer(playerClientId.ToString());
    }

    internal static bool HasItemInSlot(this PlayerControllerB player, GrabbableObject grabbable)
    {
        return player.ItemSlots.Any(slot => slot == grabbable);
    }

    internal static bool HasFreeSlots(this PlayerControllerB player)
    {
        return player.ItemSlots.Any(slot => slot == null);
    }

    internal static bool IsHoldingGrabbable(this PlayerControllerB player, GrabbableObject grabbable)
    {
        return player.ItemSlots[player.currentItemSlot] == grabbable;
    }

    internal static bool IsHoldingItemOfType<T>(this PlayerControllerB player) where T : GrabbableObject
    {
        return player.ItemSlots[player.currentItemSlot] is T;
    }

    internal static int GetSlotOfItem(this PlayerControllerB player, GrabbableObject grabbable)
    {
        return Array.IndexOf(player.ItemSlots, grabbable);
    }

    internal static bool GrabObject(this PlayerControllerB player, GrabbableObject grabbable)
    {
        if (!player.HasFreeSlots()) return false;
        NetworkObjectReference networkObject = grabbable.NetworkObject;
        _ = player.Reflect().InvokeInternalMethod("GrabObjectServerRpc", networkObject);

        grabbable.parentObject = player.localItemHolder;
        grabbable.GrabItemOnClient();

        return true;
    }

    /// <summary>
    ///     Finds the Held object slot, and discards it properly and updates the HUD slots, along with detaching if it bugs
    ///     onto the player hand.
    /// </summary>
    internal static void DiscardObject(this PlayerControllerB localPlayer, GrabbableObject item,
        bool placeObject = false, NetworkObject? parentObjectTo = null, Vector3 placePosition = default,
        bool matchRotationOfParent = true)
    {
        if (!localPlayer.IsHoldingGrabbable(item)) return;
        var slot = localPlayer.GetSlotOfItem(item);
        if (slot == -1) return;
        _ = localPlayer.Reflect().InvokeInternalMethod("SwitchToItemSlot", slot, null);
        localPlayer.DiscardHeldObject(placeObject, parentObjectTo, placePosition, matchRotationOfParent);
        item.Detach();
        RemoveItemFromHud(slot);
    }

    internal static bool IsDead(this PlayerControllerB instance)
    {
        return !instance.isPlayerControlled;
    }

    internal static PlayerControllerB? GetPlayerFromBody(this RagdollGrabbableObject body)
    {
        return GetPlayer(body.bodyID.Value);
    }
}