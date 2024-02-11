using Hax;
using UnityEngine;

internal interface IShipDoor { }

internal static class IShipDoorMixin {
    static InteractTrigger? GetAnimationInteractTrigger(this GameObject gameObject, string animation) =>
        gameObject
            .GetComponentsInChildren<AnimatedObjectTrigger>()
            .First(trigger => trigger.animationString == animation)?
            .GetComponentInParent<InteractTrigger>();

    internal static void SetShipDoorState(this IShipDoor _, bool closed) =>
        Helper.FindObject<HangarShipDoor>()?
              .gameObject.GetAnimationInteractTrigger(closed ? "CloseDoor" : "OpenDoor")?
              .onInteract.Invoke(Helper.LocalPlayer);
}
