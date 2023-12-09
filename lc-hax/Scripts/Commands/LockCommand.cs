using System.Linq;
using UnityEngine;

namespace Hax;

public class LockCommand : ICommand {
    public void Execute(string[] args) {
        ParallelQuery<DoorLock> doors = Object.FindObjectsOfType<DoorLock>().ToList().AsParallel();

        _ = Helper.CreateComponent<TransientBehaviour>().Init((_) => {
            doors.ForAll(door => {
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
