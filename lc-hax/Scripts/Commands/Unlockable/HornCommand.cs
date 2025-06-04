using System;
using System.Threading;
using System.Threading.Tasks;

[Command("horn")]
class HornCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: horn <duration>");
            return;
        }

        if (!ulong.TryParse(args[0], out ulong hornDurationSeconds)) {
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
            await Task.Delay(TimeSpan.FromSeconds(hornDurationSeconds), cancellationToken);
        }

        finally {
            shipAlarmCord.StopPullingCordServerRpc(-1);
        }
    }
}
