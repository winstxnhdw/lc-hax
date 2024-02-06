using System;
using Hax;

[Command("/horn")]
internal class HornCommand : ICommand {
    Action PullHornLater(int hornDuration) => () => {
        ShipAlarmCord? shipAlarmCord = Helper.FindObject<ShipAlarmCord>();
        shipAlarmCord?.PullCordServerRpc(-1);

        Helper.CreateComponent<WaitForBehaviour>("Stop Pulling Horn")
              .SetPredicate(time => time >= hornDuration)
              .Init(() => shipAlarmCord?.StopPullingCordServerRpc(-1));
    };

    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /horn <duration>");
            return;
        }

        if (!int.TryParse(args[0], out int hornDuration)) {
            Chat.Print("Invalid duration!");
            return;
        }

        Helper.BuyUnlockable(Unlockable.LOUD_HORN);
        Helper.ReturnUnlockable(Unlockable.LOUD_HORN);

        Helper.CreateComponent<WaitForBehaviour>("Pull Horn")
              .SetPredicate(time => time >= 0.5f)
              .Init(this.PullHornLater(hornDuration));
    }
}
