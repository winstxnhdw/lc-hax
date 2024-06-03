#region

using Hax;

#endregion

interface IShipDoor {
}

static class ShipDoorMixin {
    static HangarShipDoor? HangarShipDoor { get; set; }

    internal static void SetShipDoorState(this IShipDoor _, bool closed) {
        HangarShipDoor ??= Helper.FindObject<HangarShipDoor>();

        HangarShipDoor?
            .gameObject
            .GetComponentsInChildren<InteractTrigger>()
            .First(trigger => trigger.animationString == (closed ? "CloseDoor" : "OpenDoor"))?
            .GetComponentInParent<InteractTrigger>()
            .onInteract
            .Invoke(Helper.LocalPlayer);
    }
}
