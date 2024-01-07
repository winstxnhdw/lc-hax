using Hax;

[DebugCommand("/scraps")]
public class ScrapsCommand : ICommand {
    public void Execute(string[] _) {
        Helper.RoundManager?.currentLevel.spawnableScrap.ForEach((i, spawnableScrap) =>
            Logger.Write($"{spawnableScrap.spawnableItem.name} = {i}")
        );
    }
}
