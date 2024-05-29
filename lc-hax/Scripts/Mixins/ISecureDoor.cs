using Hax;

internal interface ISecureGate
{
}

internal static class SecureGateMixin
{
    private static TerminalAccessibleObject[]? TerminalAccessibleObjects { get; set; }

    internal static void SetSecureDoorState(this ISecureGate _, bool isUnlocked)
    {
        TerminalAccessibleObjects ??= Helper.FindObjects<TerminalAccessibleObject>();
        TerminalAccessibleObjects.ForEach(
            terminalObject => terminalObject.SetDoor(isUnlocked)
        );
    }
}