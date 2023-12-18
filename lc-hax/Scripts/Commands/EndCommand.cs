using GameNetcodeStuff;

namespace Hax;

public class EndCommand : ICommand {
    public void Execute(string[] args) {
        Helper.StartOfRound?.EndGameServerRpc(
            Helper.GetPlayer(args[0]).IsNotNull(out PlayerControllerB player) ? (int)player.playerClientId : -1
        );
    }
}
