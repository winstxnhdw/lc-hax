using Hax;

public interface ISecureGate { }

public static class ISecureDoorMixin {
    public static void SetSecureDoorState(this ISecureGate _, bool isUnlocked) =>
        Helper.FindObjects<TerminalAccessibleObject>()
              .ForEach(terminalObject => terminalObject.SetDoorOpenServerRpc(isUnlocked));
}
