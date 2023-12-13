using System.Linq;
using GameNetcodeStuff;

namespace Hax;

public static partial class Helper {
    public static PlayerControllerB? LocalPlayer => GameNetworkManager.Instance.localPlayerController;

    public static PlayerControllerB[]? Players => Helper.StartOfRound?.allPlayerScripts;

    public static PlayerControllerB? GetPlayer(string playerNameOrId) {
        PlayerControllerB[]? players = Helper.Players;

        return players?.FirstOrDefault(player => player.playerUsername == playerNameOrId) ??
               players?.FirstOrDefault(player => player.playerClientId.ToString() == playerNameOrId);
    }

    public static PlayerControllerB? GetPlayer(int playerId) => Helper.GetPlayer(playerId.ToString());
}
