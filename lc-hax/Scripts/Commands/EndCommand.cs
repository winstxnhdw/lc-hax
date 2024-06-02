using GameNetcodeStuff;
using Hax;

[Command("end")]
internal class EndCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (args.Length is 0)
            Helper.StartOfRound?.EndGameServerRpc(-1);

        else if (Helper.GetPlayer(args[0]) is PlayerControllerB player)
            player.playersManager.EndGameServerRpc(player.GetPlayerID());
    }
}