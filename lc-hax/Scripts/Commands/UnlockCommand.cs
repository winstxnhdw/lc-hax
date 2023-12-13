
using System.Linq;
using UnityEngine;

namespace Hax;

public class UnlockCommand : ICommand {
    InteractTrigger? GetDoorTrigger(DoorLock door) =>
        Reflector.Target(door).GetInternalField<InteractTrigger>("doorTrigger");

    void UnlockDoor(DoorLock door) {
        if (!Helper.Extant(this.GetDoorTrigger(door), out InteractTrigger doorTrigger)) return;

        door.UnlockDoorSyncWithServer();
        doorTrigger.timeToHold = 0.0f;
    }

    public void Execute(string[] _) {
        Object.FindObjectsOfType<DoorLock>().ToList().ForEach(this.UnlockDoor);
        Object.FindObjectsOfType<TerminalAccessibleObject>()
              .ToList()
              .ForEach(terminalAccessibleObject => terminalAccessibleObject.SetDoorOpenServerRpc(true));

        Helper.PrintSystem("All doors unlocked!");
    }
}
