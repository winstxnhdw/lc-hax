using System;
using System.Threading;
using System.Threading.Tasks;

[Command("horn")]
class HornCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: horn <duration>");
            return;
        }

        if (!int.TryParse(args[0], out int hornDurationSeconds)) {
            Chat.Print("Invalid duration!");
            return;
        }

        Helper.BuyUnlockable(Unlockable.LOUD_HORN);
        Helper.ReturnUnlockable(Unlockable.LOUD_HORN);

        if (await Helper.FindObject<ShipAlarmCord>(cancellationToken) is not ShipAlarmCord shipAlarmCord) {
            return;
        }

        try {
            shipAlarmCord.PullCordServerRpc(-1);
            await Task.Delay(hornDurationSeconds * 1000, cancellationToken);
        }

        catch (Exception) {
            throw;
        }

        finally {
            shipAlarmCord.StopPullingCordServerRpc(-1);
        }
    }
}
