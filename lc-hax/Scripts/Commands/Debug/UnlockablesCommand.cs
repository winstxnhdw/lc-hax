using System.Threading;
using System.Threading.Tasks;

[DebugCommand("unlockables")]
class UnlockablesCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        Helper.StartOfRound?.unlockablesList.unlockables.ForEach((i, unlockable) =>
            Logger.Write($"{unlockable.unlockableName} = {i}")
        );
    }
}
