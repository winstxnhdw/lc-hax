
using UnityEngine;

namespace Hax;

public class UnlockCommand : ICommand {
    public void Execute(string[] _) {
        Helper.SetGateState(true);
        Object.FindObjectsOfType<DoorLock>()
              .ForEach(door => door.UnlockDoorSyncWithServer());

        Console.Print("All doors unlocked!");
    }
}
