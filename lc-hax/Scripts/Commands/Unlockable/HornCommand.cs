using System;
using Hax;

[Command("horn")]
internal class HornCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (args.Length is 0)
        {
            Chat.Print("Usage: horn <duration>");
            return;
        }

        if (!int.TryParse(args[0], out var hornDuration))
        {
            Chat.Print("Invalid duration!");
            return;
        }

        Helper.BuyUnlockable(Unlockable.LOUD_HORN);
        Helper.ReturnUnlockable(Unlockable.LOUD_HORN);
        Helper.ShortDelay(PullHornLater(hornDuration));
    }

    private Action PullHornLater(int hornDuration)
    {
        return () =>
        {
            var shipAlarmCord = Helper.FindObject<ShipAlarmCord>();
            shipAlarmCord?.PullCordServerRpc(-1);

            Helper.CreateComponent<WaitForBehaviour>("Stop Pulling Horn")
                .SetPredicate(time => time >= hornDuration)
                .Init(() => shipAlarmCord?.StopPullingCordServerRpc(-1));
        };
    }
}