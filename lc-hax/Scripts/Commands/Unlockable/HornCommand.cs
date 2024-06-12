#region

using System;
using Hax;

#endregion

[Command("horn")]
class HornCommand : ICommand {
    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: horn <duration>");
            return;
        }

        if (!int.TryParse(args[0], out int hornDuration)) {
            Chat.Print("Invalid duration!");
            return;
        }

        Helper.BuyUnlockable("Loud horn");
        Helper.ReturnUnlockable("Loud horn");
        Helper.ShortDelay(this.PullHornLater(hornDuration));
    }

    Action PullHornLater(int hornDuration) =>
        () => {
            ShipAlarmCord? shipAlarmCord = Helper.FindObject<ShipAlarmCord>();
            shipAlarmCord?.PullCordServerRpc(-1);

            Helper.CreateComponent<WaitForBehaviour>("Stop Pulling Horn")
                .SetPredicate(time => time >= hornDuration)
                .Init(() => shipAlarmCord?.StopPullingCordServerRpc(-1));
        };
}
