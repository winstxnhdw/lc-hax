using GameNetcodeStuff;

namespace Hax;

[Command("/end")]
public class EndCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Helper.StartOfRound?.EndGameServerRpc(-1);
        }

        else if (Helper.GetPlayer(args[0]).IsNotNull(out PlayerControllerB player)) {
            Helper.StartOfRound?.EndGameServerRpc((int)player.playerClientId);
        }
    }
}
