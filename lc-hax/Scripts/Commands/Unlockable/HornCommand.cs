using System.Threading;
using System.Threading.Tasks;
using System;
using Hax;

[Command("horn")]
class HornCommand : ICommand {
    Action PullHornLater(int hornDuration) => () => {
        ShipAlarmCord? shipAlarmCord = Helper.FindObject<ShipAlarmCord>();
        shipAlarmCord?.PullCordServerRpc(-1);

        Helper.CreateComponent<WaitForBehaviour>("Stop Pulling Horn")
              .SetPredicate(time => time >= hornDuration)
              .Init(() => shipAlarmCord?.StopPullingCordServerRpc(-1));
    };

    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: horn <duration>");
            return;
        }

        if (!int.TryParse(args[0], out int hornDuration)) {
            Chat.Print("Invalid duration!");
            return;
        }

        Helper.BuyUnlockable(Unlockable.LOUD_HORN);
        Helper.ReturnUnlockable(Unlockable.LOUD_HORN);
        Helper.ShortDelay(this.PullHornLater(hornDuration));
    }
}
