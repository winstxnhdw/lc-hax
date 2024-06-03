#pragma warning disable CS8625

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace Hax
{
    internal static partial class Helper
    {
        /// <summary>
        /// Gets the local player.
        /// </summary>
        internal static PlayerControllerB? LocalPlayer => GameNetworkManager?.localPlayerController.Unfake();

        /// <summary>
        /// Gets the host player.
        /// </summary>
        internal static PlayerControllerB? HostPlayer => Players[0];

        /// <summary>
        /// Gets all players (including non-initialized players & dead players).
        /// </summary>
        internal static PlayerControllerB[] Players => StartOfRound?.allPlayerScripts ?? [];

        /// <summary>
        /// Gets the active players (players that are not dead).
        /// </summary>
        internal static PlayerControllerB[] ActivePlayers =>
            Players.Where(player => player.isPlayerControlled && !player.isPlayerDead).ToArray();

        /// <summary>
        /// Gets the player ID.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>The player ID as an integer.</returns>
        internal static int GetPlayerID(this PlayerControllerB player)
        {
            if (player == null) return -1;
            if (player.IsSelf())
            {
                return unchecked((int)player.actualClientId);
            }

            return unchecked((int)player.playerClientId);
        }

        /// <summary>
        /// Gets the player ID as a string.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>The player ID as a string.</returns>
        internal static string GetPlayerIDString(this PlayerControllerB player)
        {
            return player.GetPlayerID().ToString();
        }

        /// <summary>
        /// Determines whether the player is the local player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns><c>true</c> if the player is the local player; otherwise, <c>false</c>.</returns>
        internal static bool IsSelf(this PlayerControllerB? player)
        {
            return LocalPlayer is PlayerControllerB localPlayer && player?.actualClientId == localPlayer.actualClientId;
        }

        /// <summary>
        /// Damages the player using an RPC.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="damage">The damage amount.</param>
        internal static void DamagePlayerRpc(this PlayerControllerB player, int damage)
        {
            player.DamagePlayerFromOtherClientServerRpc(damage, Vector3.zero, -1);
        }

        /// <summary>
        /// Heals the player.
        /// </summary>
        /// <param name="player">The player.</param>
        internal static void HealPlayer(this PlayerControllerB player)
        {
            player.DamagePlayerRpc(-100);
        }

        /// <summary>
        /// Kills the player.
        /// </summary>
        /// <param name="player">The player.</param>
        internal static void KillPlayer(this PlayerControllerB player)
        {
            player.DamagePlayerRpc(100);
        }

        /// <summary>
        /// Teleports the player to the entrance.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="outside">If set to <c>true</c> teleports outside; otherwise, teleports inside.</param>
        internal static void EntranceTeleport(this PlayerControllerB player, bool outside)
        {
            player.TeleportPlayer(RoundManager.FindMainEntranceScript(outside).entrancePoint.position);
            player.isInsideFactory = !outside;
        }

        /// <summary>
        /// Gets a player by their name or ID.
        /// </summary>
        /// <param name="playerNameOrId">The player name or ID.</param>
        /// <returns>The player if found; otherwise, <c>null</c>.</returns>
        internal static PlayerControllerB? GetPlayer(string? playerNameOrId)
        {
            if (string.IsNullOrEmpty(playerNameOrId)) return null;

            var players = Players;

            return players.First(player =>
                       player.playerUsername.ToLower().Contains(playerNameOrId.ToLower(),
                           StringComparison.InvariantCultureIgnoreCase)) ??
                   players.First(player => player.GetPlayerIDString() == playerNameOrId);
        }

        /// <summary>
        /// Gets a player by their client ID.
        /// </summary>
        /// <param name="playerClientId">The player client ID.</param>
        /// <returns>The player if found; otherwise, <c>null</c>.</returns>
        internal static PlayerControllerB? GetPlayer(ulong playerClientId)
        {
            return Players.First(player => player.playerClientId == playerClientId);
        }

        /// <summary>
        /// Gets a player by their client ID.
        /// </summary>
        /// <param name="playerClientId">The player client ID.</param>
        /// <returns>The player if found; otherwise, <c>null</c>.</returns>
        internal static PlayerControllerB? GetPlayer(int playerClientId)
        {
            return GetPlayer(unchecked((ulong)playerClientId));
        }

        /// <summary>
        /// Gets an active player by their name or ID.
        /// </summary>
        /// <param name="playerNameOrId">The player name or ID.</param>
        /// <returns>The active player if found; otherwise, <c>null</c>.</returns>
        internal static PlayerControllerB? GetActivePlayer(string? playerNameOrId)
        {
            var player = GetPlayer(playerNameOrId);
            return player == null || player.IsDead() ? null : player;
        }

        /// <summary>
        /// Gets an active player by their client ID.
        /// </summary>
        /// <param name="playerClientId">The player client ID.</param>
        /// <returns>The active player if found; otherwise, <c>null</c>.</returns>
        internal static PlayerControllerB? GetActivePlayer(int playerClientId)
        {
            return GetActivePlayer(playerClientId.ToString());
        }

        /// <summary>
        /// Checks if the player has a specific item in their slot.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="grabbable">The grabbable object.</param>
        /// <returns><c>true</c> if the player has the item in their slot; otherwise, <c>false</c>.</returns>
        internal static bool HasItemInSlot(this PlayerControllerB player, GrabbableObject grabbable)
        {
            return player.ItemSlots.Any(slot => slot == grabbable);
        }

        /// <summary>
        /// Checks if the player has free slots.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns><c>true</c> if the player has free slots; otherwise, <c>false</c>.</returns>
        internal static bool HasFreeSlots(this PlayerControllerB player)
        {
            return player.ItemSlots.Any(slot => slot == null);
        }

        /// <summary>
        /// Checks if the player is holding a specific grabbable object.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="grabbable">The grabbable object.</param>
        /// <returns><c>true</c> if the player is holding the grabbable object; otherwise, <c>false</c>.</returns>
        internal static bool IsHoldingGrabbable(this PlayerControllerB player, GrabbableObject grabbable)
        {
            return player.ItemSlots[player.currentItemSlot] == grabbable;
        }

        /// <summary>
        /// Checks if the player is holding an item of a specific type.
        /// </summary>
        /// <typeparam name="T">The type of the grabbable object.</typeparam>
        /// <param name="player">The player.</param>
        /// <returns><c>true</c> if the player is holding an item of the specified type; otherwise, <c>false</c>.</returns>
        internal static bool IsHoldingItemOfType<T>(this PlayerControllerB player) where T : GrabbableObject
        {
            return player.ItemSlots[player.currentItemSlot] is T;
        }

        /// <summary>
        /// Gets the slot of a specific item.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="grabbable">The grabbable object.</param>
        /// <returns>The slot index of the item.</returns>
        internal static int GetSlotOfItem(this PlayerControllerB player, GrabbableObject grabbable)
        {
            return Array.IndexOf(player.ItemSlots, grabbable);
        }

        /// <summary>
        /// Grabs an object.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="grabbable">The grabbable object.</param>
        /// <returns><c>true</c> if the object was successfully grabbed; otherwise, <c>false</c>.</returns>
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
        /// Discards an object.
        /// </summary>
        /// <param name="localPlayer">The local player.</param>
        /// <param name="item">The item to discard.</param>
        /// <param name="placeObject">If set to <c>true</c>, places the object; otherwise, discards it.</param>
        /// <param name="parentObjectTo">The parent object to attach to.</param>
        /// <param name="placePosition">The position to place the object.</param>
        /// <param name="matchRotationOfParent">If set to <c>true</c>, matches the rotation of the parent object.</param>
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

        /// <summary>
        /// Determines whether the player is dead.
        /// </summary>
        /// <param name="instance">The player instance.</param>
        /// <returns><c>true</c> if the player is dead; otherwise, <c>false</c>.</returns>
        internal static bool IsDead(this PlayerControllerB instance) => instance.isPlayerDead;

        /// <summary>
        /// Determines whether the player is controlled.
        /// </summary>
        /// <param name="instance">The player instance.</param>
        /// <returns><c>true</c> if the player is controlled; otherwise, <c>false</c>.</returns>
        internal static bool IsControlled(this PlayerControllerB instance) => instance.isPlayerControlled;

        /// <summary>
        /// Determines whether the player is Deactivated (dead and not controlled).
        /// </summary>
        /// <param name="instance">The player instance.</param>
        /// <returns><c>true</c> if the player is Deactivated; otherwise, <c>false</c>.</returns>
        internal static bool isDeactivatedPlayer(this PlayerControllerB instance) => instance.IsDead() && !instance.IsControlled();

        /// <summary>
        /// Gets a player from a body.
        /// </summary>
        /// <param name="body">The ragdoll grabbable object representing the body.</param>
        /// <returns>The player if found; otherwise, <c>null</c>.</returns>
        internal static PlayerControllerB? GetPlayerFromBody(this RagdollGrabbableObject body) => GetPlayer(body.bodyID.Value);

        /// <summary>
        /// Retrieves the username of the specified player.
        /// </summary>
        /// <param name="player">The player controller.</param>
        /// <returns>The username of the player, or <c>null</c> if the player is not initialized or the username is not set.</returns>
        internal static string GetPlayerUsername(this PlayerControllerB player)
        {
            if (player == null) return null;
            if (player.playerUsername == "Player") return null; // means not initialized
            return player.playerUsername;
        }
    }
}
