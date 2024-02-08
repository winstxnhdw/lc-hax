using Hax;

[Command("/explode")]
internal class ExplodeCommand : ICommand {
    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Helper.FindObjects<JetpackItem>()
                  .ForEach(jetpack => jetpack.ExplodeJetpackServerRpc());
        }

        else if (args[0] is "mine") {
            Helper.FindObjects<Landmine>()
                  .ForEach(landmine => landmine.TriggerMine());
        }
    }
}
