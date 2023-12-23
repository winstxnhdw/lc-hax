using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static Terminal? Terminal => Helper.HUDManager?.Reflect().GetInternalField<Terminal>("terminalScript");

    public static void SetGateState(bool isUnlocked) =>
        Object.FindObjectsOfType<TerminalAccessibleObject>()
              .ForEach(terminalObject => terminalObject.SetDoorOpenServerRpc(isUnlocked));
}
