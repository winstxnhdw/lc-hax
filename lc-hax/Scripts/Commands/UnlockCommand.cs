
using System.Linq;
using UnityEngine;

namespace Hax;

public class UnlockCommand : ICommand {
    public void Execute(string[] _) {
        Object.FindObjectsOfType<DoorLock>().ToList().ForEach(door => {
            door.UnlockDoorSyncWithServer();

            if (!Helpers.Extant(Reflector.Target(door).GetInternalField<InteractTrigger>("doorTrigger"), out InteractTrigger doorTrigger)) {
                return;
            }

            doorTrigger.timeToHold = 0.0f;
        });

        Console.Print("SYSTEM", "All doors unlocked!");
    }
}
