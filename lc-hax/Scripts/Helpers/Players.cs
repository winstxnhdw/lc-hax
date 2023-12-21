using System.Linq;
using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public static partial class Helper {
    public static void KillPlayer(this PlayerControllerB player) =>
        player.DamagePlayerFromOtherClientServerRpc(1000, Vector3.zero, -1);

    public static PlayerControllerB? LocalPlayer => GameNetworkManager.Instance.localPlayerController;

    public static PlayerControllerB[]? Players => Helper.StartOfRound?.allPlayerScripts;

    public static PlayerControllerB? GetPlayer(int playerClientId) => Helper.Players?.FirstOrDefault(player => player.playerClientId == (ulong)playerClientId);

    public static PlayerControllerB? GetPlayer(string username) => Helper.Players?.FirstOrDefault(player => player.playerUsername == username);

    public static PlayerControllerB? GetActivePlayer(string username) =>
        Helper.GetPlayer(username).IsNotNull(out PlayerControllerB player)
            ? player.isPlayerDead ? null : !player.isPlayerControlled ? null : player
            : null;

    public static PlayerControllerB? GetActivePlayer(int playerClientId) =>
        Helper.GetPlayer(playerClientId).IsNotNull(out PlayerControllerB player)
            ? player.isPlayerDead ? null : !player.isPlayerControlled ? null : player
            : null;
}
