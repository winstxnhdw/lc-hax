
using System.Linq;
using UnityEngine;

namespace Hax;

public class UnlockCommand : ICommand {
    public void Execute(string[] _) {
        Object.FindObjectsOfType<DoorLock>().ToList().ForEach(door => {
            door.UnlockDoorSyncWithServer();

            InteractTrigger? doorTrigger = Reflector.Target(door).GetInternalField<InteractTrigger>("doorTrigger");

            if (doorTrigger == null) {
                return;
            }

            doorTrigger.timeToHold = 0.0f;
        });

        Console.Print("SYSTEM", "All doors unlocked!");
    }
}
