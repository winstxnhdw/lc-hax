using System.Threading;
using System.Threading.Tasks;

[DebugCommand("unlockables")]
sealed class UnlockablesCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        Helper.StartOfRound?.unlockablesList.unlockables.ForEach((i, unlockable) =>
            Logger.Write($"{unlockable.unlockableName} = {i}")
        );

        return Task.CompletedTask;
    }
}
