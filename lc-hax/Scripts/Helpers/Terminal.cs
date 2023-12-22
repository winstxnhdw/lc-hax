using System.Linq;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static Terminal? Terminal => Helper.HUDManager?.Reflect().GetInternalField<Terminal>("terminalScript");

    public static void SetGateState(bool isUnlocked) =>
        Object.FindObjectsOfType<TerminalAccessibleObject>()
              .ToList()
              .ForEach(terminalObject => terminalObject.SetDoorOpenServerRpc(isUnlocked));
}
