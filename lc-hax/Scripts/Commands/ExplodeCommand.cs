using System.Threading;
using System.Threading.Tasks;

[Command("explode")]
class ExplodeCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) {
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
