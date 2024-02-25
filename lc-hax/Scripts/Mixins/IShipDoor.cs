using Hax;
using UnityEngine;

interface IShipDoor { }

static class ShipDoorMixin {
    static HangarShipDoor? HangarShipDoor { get; set; }

    internal static void SetShipDoorState(this IShipDoor _, bool closed) {
        ShipDoorMixin.HangarShipDoor ??= Helper.FindObject<HangarShipDoor>();

        ShipDoorMixin
            .HangarShipDoor?
            .gameObject
            .GetComponentsInChildren<InteractTrigger>()
            .First(trigger => trigger.animationString == (closed ? "CloseDoor" : "OpenDoor"))?
            .GetComponentInParent<InteractTrigger>()
            .onInteract
            .Invoke(Helper.LocalPlayer);
    }
}
