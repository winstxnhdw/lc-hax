using System.Threading;
using System.Threading.Tasks;

[DebugCommand("unlockables")]
class UnlockablesCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        Helper.StartOfRound?.unlockablesList.unlockables.ForEach((i, unlockable) =>
            Logger.Write($"{unlockable.unlockableName} = {i}")
        );
    }
}
