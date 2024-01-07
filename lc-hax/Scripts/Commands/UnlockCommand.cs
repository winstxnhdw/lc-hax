using Hax;

[Command("/unlock")]
public class UnlockCommand : ICommand {
    public void Execute(string[] _) {
        Helper.SetGateState(true);
        Helper.FindObjects<DoorLock>()
              .ForEach(door => door.UnlockDoorSyncWithServer());

        Chat.Print("All doors unlocked!");
    }
}
