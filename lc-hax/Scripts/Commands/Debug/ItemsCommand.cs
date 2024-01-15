using System;
using Hax;

[DebugCommand("/items")]
public class ItemsCommand : ICommand {
    public void Execute(ReadOnlySpan<string> _) {
        Helper.Terminal?.buyableItemsList.ForEach((i, item) =>
            Logger.Write($"{item.name} = {i}")
        );
    }
}
