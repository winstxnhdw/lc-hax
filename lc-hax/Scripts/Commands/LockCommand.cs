using System.Linq;
using UnityEngine;

namespace Hax;

public class LockCommand : ICommand {
    protected void SetGateState(bool isUnlocked) =>
        Object.FindObjectsOfType<TerminalAccessibleObject>()
              .ToList()
              .ForEach(terminalObject => terminalObject.SetDoorOpenServerRpc(isUnlocked));

    public void Execute(string[] args) {
        this.SetGateState(false);
    }
}
