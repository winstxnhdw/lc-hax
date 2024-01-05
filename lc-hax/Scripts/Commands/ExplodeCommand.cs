using UnityEngine;

namespace Hax;

[Command("/explode")]
public class ExplodeCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Object.FindObjectsByType<JetpackItem>(FindObjectsSortMode.None)
                  .ForEach(jetpack => jetpack.ExplodeJetpackServerRpc());
        }

        if (args[0] is "mine") {
            Object.FindObjectsByType<Landmine>(FindObjectsSortMode.None)
                  .ForEach(landmine => landmine.TriggerMine());
        }
    }
}
