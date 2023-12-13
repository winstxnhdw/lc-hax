using System.Linq;
using UnityEngine;

namespace Hax;

public class LockCommand : ICommand {
    public void Execute(string[] args) {
        Object.FindObjectsOfType<TerminalAccessibleObject>()
              .ToList()
              .ForEach(terminalObject => terminalObject.SetDoorOpenServerRpc(false));
    }
}
