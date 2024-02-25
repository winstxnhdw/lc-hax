using Hax;

[DebugCommand("scraps")]
class ScrapsCommand : ICommand {
    public void Execute(StringArray _) {
        Helper.RoundManager?.currentLevel.spawnableScrap.ForEach((i, spawnableScrap) =>
            Logger.Write($"{spawnableScrap.spawnableItem.name} = {i}")
        );
    }
}
