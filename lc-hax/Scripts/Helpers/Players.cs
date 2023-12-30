using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public static partial class Helper {
    static void DamagePlayer(this PlayerControllerB player, int damage) =>
        player.DamagePlayerFromOtherClientServerRpc(damage, Vector3.zero, -1);

    public static void HealPlayer(this PlayerControllerB player) => player.DamagePlayer(-100);

    public static void KillPlayer(this PlayerControllerB player) => player.DamagePlayer(100);

    public static PlayerControllerB? LocalPlayer => GameNetworkManager.Instance.localPlayerController;

    public static PlayerControllerB[] Players => Helper.StartOfRound?.allPlayerScripts ?? [];

    public static PlayerControllerB? GetPlayer(string playerNameOrId) {
        PlayerControllerB[] players = Helper.Players;

        return players.First(player => player.playerUsername == playerNameOrId) ??
               players.First(player => player.playerClientId.ToString() == playerNameOrId);
    }

    public static PlayerControllerB? GetPlayer(int playerClientId) => Helper.Players.First(player => player.playerClientId == (ulong)playerClientId);

    public static PlayerControllerB? GetActivePlayer(string playerNameOrId) =>
        Helper.GetPlayer(playerNameOrId).IsNotNull(out PlayerControllerB player)
            ? player.isPlayerDead ? null : !player.isPlayerControlled ? null : player
            : null;

    public static PlayerControllerB? GetActivePlayer(int playerClientId) =>
        Helper.GetPlayer(playerClientId).IsNotNull(out PlayerControllerB player)
            ? player.isPlayerDead ? null : !player.isPlayerControlled ? null : player
            : null;
}
