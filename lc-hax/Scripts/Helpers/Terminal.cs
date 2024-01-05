namespace Hax;

public static partial class Helper {
    public static Terminal? Terminal => Helper.HUDManager?.Reflect().GetInternalField<Terminal>("terminalScript");

    public static void SetGateState(bool isUnlocked) =>
        Helper.FindObjects<TerminalAccessibleObject>()
              .ForEach(terminalObject => terminalObject.SetDoorOpenServerRpc(isUnlocked));
}
