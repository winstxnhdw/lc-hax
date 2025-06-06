using System.Runtime.CompilerServices;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using ZLinq;

static partial class Helper {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int PlayerIndex(this PlayerControllerB player) => unchecked((int)player.playerClientId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsSelf(this PlayerControllerB? player) => Helper.LocalPlayer is PlayerControllerB localPlayer && player?.actualClientId == localPlayer.actualClientId;

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

    internal static PlayerControllerB[] ActivePlayers => Helper.Players.AsValueEnumerable().Where(player => player.isPlayerControlled && !player.isPlayerDead).ToArray();

    internal static PlayerControllerB? GetPlayer(string? playerNameOrId) {
        if (string.IsNullOrEmpty(playerNameOrId)) return null;

        PlayerControllerB[] players = Helper.Players;

        return players.First(player => player.playerUsername == playerNameOrId) ??
               players.First(player => player.playerClientId.ToString() == playerNameOrId);
    }

    internal static PlayerControllerB? GetPlayer(ulong playerClientId) => Helper.Players.First(player => player.playerClientId == playerClientId);

    internal static PlayerControllerB? GetPlayer(int playerClientId) => Helper.GetPlayer(unchecked((ulong)playerClientId));

    internal static PlayerControllerB? GetActivePlayer(string? playerNameOrId) =>
        Helper.GetPlayer(playerNameOrId) is not PlayerControllerB player || !player.isPlayerControlled || player.isPlayerDead
            ? null
            : player;

    internal static PlayerControllerB? GetActivePlayer(int playerClientId) => Helper.GetActivePlayer(playerClientId.ToString());

    internal static bool GrabObject(this PlayerControllerB player, GrabbableObject grabbable) {
        if (player.ItemSlots.WhereIsNotNull().AsValueEnumerable().Count() >= 4) return false;

        NetworkObjectReference networkObject = grabbable.NetworkObject;
        player.GrabObjectServerRpc(networkObject);

        grabbable.parentObject = player.localItemHolder;
        grabbable.GrabItemOnClient();

        return true;
    }
}
