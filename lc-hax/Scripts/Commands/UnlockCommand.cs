namespace Hax;

[Command("/unlock")]
public class UnlockCommand : ICommand {
    public void Execute(string[] _) {
        Helper.SetGateState(true);
        Helper.FindObjects<DoorLock>()
              .ForEach(door => door.UnlockDoorSyncWithServer());

        Console.Print("All doors unlocked!");
    }
}
