
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hax;

public class LockCommand : ICommand {
    public void Execute(string[] args) {
        List<DoorLock> doors = [.. Object.FindObjectsOfType<DoorLock>()];

        _ = Helper.CreateComponent<TransientBehaviour>().Init((_) => {
            doors.ForEach(door => {
                AnimatedObjectTrigger animatedObjectTrigger = door.gameObject.GetComponent<AnimatedObjectTrigger>();
                animatedObjectTrigger.boolValue = false;
                animatedObjectTrigger.TriggerAnimation(Helper.GetPlayer(0));
            });
        }, 60.0f);

        Object.FindObjectsOfType<TerminalAccessibleObject>()
              .ToList()
              .ForEach(terminalAccessibleObject => terminalAccessibleObject.SetDoorOpenServerRpc(false));

    }
}
