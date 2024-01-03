using UnityEngine;

namespace Hax;

public static partial class Helper {
    static InteractTrigger? GetInteractTriggerForAnimation(this GameObject? gameObject, string animation) =>
        gameObject.Unfake()?
            .GetComponentsInChildren<AnimatedObjectTrigger>()
            .First(trigger => trigger.animationString == animation)?
            .GetComponentInParent<InteractTrigger>();

    public static void CloseShipDoor(bool closed) =>
        Object.FindObjectOfType<HangarShipDoor>()
              .gameObject.GetInteractTriggerForAnimation(closed ? "CloseDoor" : "OpenDoor")?
              .onInteract.Invoke(Helper.LocalPlayer);
}
