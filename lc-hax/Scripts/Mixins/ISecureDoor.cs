using Hax;

interface ISecureGate { }

static class SecureGateMixin {
    static TerminalAccessibleObject[]? TerminalAccessibleObjects { get; set; }

    internal static void SetSecureDoorState(this ISecureGate _, bool isUnlocked) {
        SecureGateMixin.TerminalAccessibleObjects ??= Helper.FindObjects<TerminalAccessibleObject>();
        SecureGateMixin.TerminalAccessibleObjects.ForEach(
            terminalObject => terminalObject.SetDoorOpenServerRpc(isUnlocked)
        );
    }
}
