using UnityEngine;

namespace Hax;

public class ExplodeCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Object.FindObjectsOfType<Landmine>()
                  .ForEach(mine => mine.TriggerMine());
        }

        if (args[0] is "jet") {
            Object.FindObjectsOfType<JetpackItem>()
                  .ForEach(jetpack => jetpack.ExplodeJetpackServerRpc());
        }
    }
}
