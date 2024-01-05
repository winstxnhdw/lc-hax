using System;

namespace Hax;

[Command("/horn")]
public class HornCommand : ICommand {
    Action PullHornLater(int hornDuration) => () => {
        ShipAlarmCord shipAlarmCord = Helper.FindObject<ShipAlarmCord>();
        shipAlarmCord.PullCordServerRpc(-1);

        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(time => time >= hornDuration)
              .Init(() => shipAlarmCord.StopPullingCordServerRpc(-1));
    };

    public void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /horn <duration>");
            return;
        }

        if (!int.TryParse(args[0], out int hornDuration)) {
            Console.Print("Invalid duration!");
            return;
        }

        Helper.BuyUnlockable(Unlockable.LOUD_HORN);
        Helper.ReturnUnlockable(Unlockable.LOUD_HORN);

        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(time => time >= 0.5f)
              .Init(this.PullHornLater(hornDuration));
    }
}
