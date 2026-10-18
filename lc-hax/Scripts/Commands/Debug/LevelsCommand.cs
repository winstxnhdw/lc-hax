using System.Threading;
using System.Threading.Tasks;

[DebugCommand("levels")]
sealed class LevelsCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        Helper.StartOfRound?.levels.ForEach((i, level) =>
            Logger.Write($"{level.name} = {i}")
        );

        return Task.CompletedTask;
    }
}
