using System.Linq;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static InteractTrigger? GetInteractTriggerForAnimation(GameObject? gameObject, string animation) {
        return gameObject.Unfake()?
            .GetComponentsInChildren<AnimatedObjectTrigger>()
            .FirstOrDefault(trigger => trigger.animationString == animation)?
            .GetComponentInParent<InteractTrigger>();
    }

    public static void CloseShipDoor(bool closed) {
        if (!Object.FindObjectOfType<HangarShipDoor>().IsNotNull(out HangarShipDoor shipDoor)) return;

        GetInteractTriggerForAnimation(shipDoor.gameObject, closed ? "CloseDoor" : "OpenDoor")?.onInteract.Invoke(Helper.LocalPlayer);
    }
}
