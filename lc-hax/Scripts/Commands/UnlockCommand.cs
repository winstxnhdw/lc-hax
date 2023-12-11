
using System.Linq;
using UnityEngine;

namespace Hax;

public class UnlockCommand : ICommand {
    public void Execute(string[] _) {
        Object.FindObjectsOfType<DoorLock>().ToList().ForEach(door => {
            door.UnlockDoorSyncWithServer();

            if (!Helper.Extant(Reflector.Target(door).GetInternalField<InteractTrigger>("doorTrigger"), out InteractTrigger doorTrigger)) {
                return;
            }

            doorTrigger.timeToHold = 0.0f;
        });

        Object.FindObjectsOfType<TerminalAccessibleObject>()
              .ToList()
              .ForEach(terminalAccessibleObject => terminalAccessibleObject.SetDoorOpenServerRpc(true));

        Helper.PrintSystem("All doors unlocked!");
    }
}
