using System;
using Hax;

[Command("/horn")]
public class HornCommand : ICommand {
    Action ResetHornLater(
        ShipAlarmCord shipAlarmCord,
        Vector3 previousPosition,
        Vector3 previousRotation
    ) => () => {
        Helper.PlaceObjectAtPosition(shipAlarmCord, previousPosition, previousRotation);
        shipAlarmCord?.StopPullingCordServerRpc(-1)
    }

    Action PullHornLater(float hornDuration) => () => {
        if(!Helper.FindObject<ShipAlarmCord>().IsNotNull(out ShipAlarmCord shipAlarmCord)) return;
        
        Vector3 previousPosition = shipAlarmCord.transform.position.Copy();
        Vector3 previousRotation = shipAlarmCord.transform.eulerAngles.Copy();
        Helper.PlaceObjectAtPosition(shipAlarmCord, new Vector3(0.0f, -50.0f, 0.0f));
        shipAlarmCord.PullCordServerRpc(-1);

        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(time => time >= hornDuration)
              .Init(this.ResetHornLater(shipAlarmCord));
    };

    public void Execute(string[] args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /horn <duration>");
            return;
        }

        if (!ulong.TryParse(args[0], out ulong hornDuration)) {
            Chat.Print("Invalid duration!");
            return;
        }

        Helper.BuyUnlockable(Unlockable.LOUD_HORN);
        Helper.ReturnUnlockable(Unlockable.LOUD_HORN);

        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(time => time >= 0.5f)
              .Init(this.PullHornLater(hornDuration));
    }
}
