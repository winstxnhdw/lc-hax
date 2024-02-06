using Hax;

internal interface ISecureGate { }

internal static class ISecureDoorMixin {
    internal static void SetSecureDoorState(this ISecureGate _, bool isUnlocked) =>
        Helper.FindObjects<TerminalAccessibleObject>()
              .ForEach(terminalObject => terminalObject.SetDoorOpenServerRpc(isUnlocked));
}
