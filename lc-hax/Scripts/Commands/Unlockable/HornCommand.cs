using System;
using System.Threading;
using System.Threading.Tasks;

[Command("horn")]
sealed class HornCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: horn <duration>");
            return;
        }

        if (!ulong.TryParse(args[0], out ulong duration)) {
            Chat.Print($"Horn {nameof(duration)} must be a positive number!");
            return;
        }

        Helper.BuyUnlockable(Unlockable.LOUD_HORN);
        Helper.ReturnUnlockable(Unlockable.LOUD_HORN);

        if (await Helper.FindObject<ShipAlarmCord>(cancellationToken) is not ShipAlarmCord shipAlarmCord) {
            return;
        }

        try {
            shipAlarmCord.PullCordServerRpc(-1);
            await Task.Delay(TimeSpan.FromSeconds(duration), cancellationToken);
        }

        finally {
            shipAlarmCord.StopPullingCordServerRpc(-1);
        }
    }
}
