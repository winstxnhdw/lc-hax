using System.Linq;
using UnityEngine;

namespace Hax;

public class ExplodeCommand : ICommand {
    public void Execute(string[] _) {
        Object
            .FindObjectsOfType<Landmine>()
            .ToList()
            .ForEach(mine => Reflector.Target(mine).InvokeInternalMethod("TriggerMineOnLocalClientByExiting"));
    }
}
