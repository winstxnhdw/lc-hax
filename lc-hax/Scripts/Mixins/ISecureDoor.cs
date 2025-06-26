interface ISecureDoor;

static class SecureDoorMixin {
    static TerminalAccessibleObject[]? TerminalAccessibleObjects { get; set; }

    internal static void SetSecureDoorState(this ISecureDoor _, bool isUnlocked) {
        SecureDoorMixin.TerminalAccessibleObjects ??= Helper.FindObjects<TerminalAccessibleObject>();
        SecureDoorMixin.TerminalAccessibleObjects.ForEach(
            terminalObject => terminalObject.SetDoorOpenServerRpc(isUnlocked)
        );
    }
}
