using UnityEngine;

namespace Hax;

[Command("/explode")]
public class ExplodeCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Object.FindObjectsOfType<JetpackItem>()
                  .ForEach(jetpack => jetpack.ExplodeJetpackServerRpc());
        }

        if (args[0] is "mine") {
            Object.FindObjectsOfType<Landmine>()
                  .ForEach(landmine => landmine.TriggerMine());
        }
    }
}
