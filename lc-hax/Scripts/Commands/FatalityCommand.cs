using GameNetcodeStuff;
using Hax;

[Command("/fatality")]
public class FatalityCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length < 2) {
            Chat.Print("Usage: /fatality <player> <enemy>");
            return;
        }

        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) {
            Chat.Print("Local player not found!");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB player) {
            Chat.Print("Invalid player!");
            return;
        }

        if (args[1] is "giant") {
            ForestGiantAI? forestGiant = Helper.FindObject<ForestGiantAI>();
            forestGiant?.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
            forestGiant?.GrabPlayerServerRpc((int)player.playerClientId);
        }

        else if (args[1] is "jester") {
            JesterAI? jester = Helper.FindObject<JesterAI>();
            jester?.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
            jester?.KillPlayerServerRpc((int)player.playerClientId);
        }

        else if (args[1] is "mask") {
            MaskedPlayerEnemy? spider = Helper.FindObject<MaskedPlayerEnemy>();
            spider?.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
            spider?.KillPlayerAnimationServerRpc((int)player.playerClientId);
        }

        else {
            Chat.Print("Enemy fatality has not yet been implemented or does not exist!");
        }
    }
}
