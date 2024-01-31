using UnityEngine;
using GameNetcodeStuff;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hax;

public static partial class Helper {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ClientId(this PlayerControllerB player) => unchecked((int)player.playerClientId);

    public static void DamagePlayerRpc(this PlayerControllerB player, int damage) =>
        player.DamagePlayerFromOtherClientServerRpc(damage, Vector3.zero, -1);

    public static void HealPlayer(this PlayerControllerB player) => player.DamagePlayerRpc(-100);

    public static void KillPlayer(this PlayerControllerB player) => player.DamagePlayerRpc(100);

    public static PlayerControllerB? LocalPlayer => GameNetworkManager.Instance.localPlayerController.Unfake();

    public static PlayerControllerB[] Players => Helper.StartOfRound?.allPlayerScripts ?? [];

    public static PlayerControllerB[] ActivePlayers => Helper.Players.Where(player => player.isPlayerControlled && !player.isPlayerDead).ToArray();

    public static PlayerControllerB? GetPlayer(string playerNameOrId) {
        PlayerControllerB[] players = Helper.Players;

        return players.First(player => player.playerUsername == playerNameOrId) ??
               players.First(player => player.playerClientId.ToString() == playerNameOrId);
    }

    public static PlayerControllerB? GetPlayer(ulong playerClientId) => Helper.Players.First(player => player.playerClientId == playerClientId);

    public static PlayerControllerB? GetActivePlayer(string playerNameOrId) =>
        Helper.GetPlayer(playerNameOrId) is not PlayerControllerB player || !player.isPlayerControlled || player.isPlayerDead
            ? null
            : player;

    public static PlayerControllerB? GetActivePlayer(int playerClientId) => Helper.GetActivePlayer(playerClientId.ToString());
}
