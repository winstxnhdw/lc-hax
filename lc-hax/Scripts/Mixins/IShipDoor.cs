interface IShipDoor;

static class ShipDoorMixin {
    static HangarShipDoor? HangarShipDoor { get; set; }

    internal static void SetShipDoorState(this IShipDoor _, bool closed) {
        string targetAnimation = closed ? "CloseDoor" : "OpenDoor";
        ShipDoorMixin.HangarShipDoor ??= Helper.FindObject<HangarShipDoor>();

        ShipDoorMixin
            .HangarShipDoor?
            .GetComponentsInChildren<InteractTrigger>()
            .First(trigger => trigger.GetComponent<AnimatedObjectTrigger>().animationString == targetAnimation)?
            .onInteract
            .Invoke(Helper.LocalPlayer);
    }
}
