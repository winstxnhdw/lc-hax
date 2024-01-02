using UnityEngine;

namespace Hax;

[Command("/horn")]
public class HornCommand : ICommand {
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
        ShipAlarmCord shipAlarmCord = Object.FindObjectOfType<ShipAlarmCord>();
        shipAlarmCord.PullCordServerRpc(-1);

        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(time => time >= hornDuration)
              .Init(() => shipAlarmCord.StopPullingCordServerRpc(-1));
    }
}
