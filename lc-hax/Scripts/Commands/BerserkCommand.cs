using System.Threading;
using System.Threading.Tasks;

[Command("berserk")]
class BerserkCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) =>
        Helper.FindObjects<Turret>()
              .ForEach(turret => turret.EnterBerserkModeServerRpc(-1));
}
