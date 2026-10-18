using System.Threading;
using System.Threading.Tasks;

[DebugCommand("scraps")]
sealed class ScrapsCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        Helper.RoundManager?.currentLevel.spawnableScrap.ForEach((i, spawnableScrap) =>
            Logger.Write($"{spawnableScrap.spawnableItem.name} = {i}")
        );

        return Task.CompletedTask;
    }
}
