using System;
using Hax;

[DebugCommand("/scraps")]
public class ScrapsCommand : ICommand {
    public void Execute(ReadOnlySpan<string> _) {
        Helper.RoundManager?.currentLevel.spawnableScrap.ForEach((i, spawnableScrap) =>
            Logger.Write($"{spawnableScrap.spawnableItem.name} = {i}")
        );
    }
}
