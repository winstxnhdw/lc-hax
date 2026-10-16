interface ISecureDoor;

static class SecureDoorMixin {
    internal static void SetSecureDoorState(this ISecureDoor _, bool isUnlocked) {
        Helper.FindObjects<TerminalAccessibleObject>()
              .ForEach(terminalObject => terminalObject.SetDoorOpenServerRpc(isUnlocked));
    }
}
