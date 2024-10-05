using System.Threading;
using System.Threading.Tasks;
using Hax;

[DebugCommand("items")]
class ItemsCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        Helper.Terminal?.buyableItemsList.ForEach((i, item) =>
            Logger.Write($"{item.name} = {i}")
        );
    }
}
