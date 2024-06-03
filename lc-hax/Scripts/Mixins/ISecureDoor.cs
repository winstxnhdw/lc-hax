#region

using Hax;

#endregion

interface ISecureGate {
}

static class SecureGateMixin {
    static TerminalAccessibleObject[]? TerminalAccessibleObjects { get; set; }

    internal static void SetSecureDoorState(this ISecureGate _, bool isUnlocked) {
        TerminalAccessibleObjects ??= Helper.FindObjects<TerminalAccessibleObject>();
        TerminalAccessibleObjects.ForEach(
            terminalObject => terminalObject.SetDoor(isUnlocked)
        );
    }
}
