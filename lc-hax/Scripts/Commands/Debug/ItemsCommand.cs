using Hax;

[DebugCommand("items")]
class ItemsCommand : ICommand {
    public void Execute(StringArray _) {
        Helper.Terminal?.buyableItemsList.ForEach((i, item) =>
            Logger.Write($"{item.name} = {i}")
        );
    }
}
