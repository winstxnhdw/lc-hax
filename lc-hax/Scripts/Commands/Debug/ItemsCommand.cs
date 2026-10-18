using System.Threading;
using System.Threading.Tasks;

[DebugCommand("items")]
sealed class ItemsCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        Helper.Terminal?.buyableItemsList.ForEach((i, item) =>
            Logger.Write($"{item.name} = {i}")
        );

        return Task.CompletedTask;
    }
}
