using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("end")]
class EndCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Helper.StartOfRound?.EndGameServerRpc(-1);
        }

        else if (Helper.GetPlayer(args[0]) is PlayerControllerB player) {
            player.playersManager.EndGameServerRpc(player.PlayerIndex());
        }
    }
}
