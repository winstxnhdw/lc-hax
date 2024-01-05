using UnityEngine;

namespace Hax;

[Command("/unlock")]
public class UnlockCommand : ICommand {
    public void Execute(string[] _) {
        Helper.SetGateState(true);
        Object.FindObjectsByType<DoorLock>(FindObjectsSortMode.None)
              .ForEach(door => door.UnlockDoorSyncWithServer());

        Console.Print("All doors unlocked!");
    }
}
