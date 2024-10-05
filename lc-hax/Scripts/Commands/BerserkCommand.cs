using System.Threading;
using System.Threading.Tasks;
using Hax;

[Command("berserk")]
class BerserkCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) =>
        Helper.FindObjects<Turret>()
              .ForEach(turret => turret.EnterBerserkModeServerRpc(-1));
}
