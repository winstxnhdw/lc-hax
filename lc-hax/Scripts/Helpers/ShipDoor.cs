using UnityEngine;

namespace Hax;

public static partial class Helper {
    static InteractTrigger? GetAnimationInteractTrigger(this GameObject gameObject, string animation) =>
        gameObject
            .GetComponentsInChildren<AnimatedObjectTrigger>()
            .First(trigger => trigger.animationString == animation)?
            .GetComponentInParent<InteractTrigger>();

    public static void CloseShipDoor(bool closed) =>
        Helper.FindObject<HangarShipDoor>()
              .gameObject.GetAnimationInteractTrigger(closed ? "CloseDoor" : "OpenDoor")?
              .onInteract.Invoke(Helper.LocalPlayer);
}
