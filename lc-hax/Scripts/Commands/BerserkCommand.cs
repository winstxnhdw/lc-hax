using System.Threading;
using System.Threading.Tasks;

[Command("berserk")]
sealed class BerserkCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        Helper.FindObjects<Turret>()
              .ForEach(turret => turret.EnterBerserkModeServerRpc(-1));
        return Task.CompletedTask;
    }
}
