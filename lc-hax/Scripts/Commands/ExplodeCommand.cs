using System;
using Hax;

[Command("/explode")]
public class ExplodeCommand : ICommand {
    public void Execute(ReadOnlySpan<string> args) {
        if (args.Length is 0) {
            Helper.FindObjects<JetpackItem>()
                  .ForEach(jetpack => jetpack.ExplodeJetpackServerRpc());
        }

        if (args[0] is "mine") {
            Helper.FindObjects<Landmine>()
                  .ForEach(landmine => landmine.TriggerMine());
        }
    }
}
