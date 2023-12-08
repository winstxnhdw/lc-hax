using System;
using System.Linq;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public static partial class Helpers {
    public static PlayerControllerB? LocalPlayer => GameNetworkManager.Instance.localPlayerController;

    public static PlayerControllerB[]? Players => StartOfRound.Instance.allPlayerScripts;

    public static PlayerControllerB GetPlayer(string playerNameOrId) {
        PlayerControllerB[]? players = Helpers.Players;

        return players?.FirstOrDefault(player => player.playerUsername == playerNameOrId) ??
              (players?.FirstOrDefault(player => player.playerClientId.ToString() == playerNameOrId)) ??
              throw new Exception("Player not found!");
    }


}
