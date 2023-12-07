using System.Linq;
using UnityEngine;

namespace Hax;

public class ExplodeCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length < 1) {
            Object
                .FindObjectsOfType<Landmine>()
                .ToList()
                .ForEach(mine => Reflector.Target(mine).InvokeInternalMethod("TriggerMineOnLocalClientByExiting"));
        }

        if (args[0] is "jet") {
            Object
                .FindObjectsOfType<JetpackItem>()
                .ToList()
                .ForEach(jetpack => jetpack.ExplodeJetpackServerRpc());
        }
    }
}
