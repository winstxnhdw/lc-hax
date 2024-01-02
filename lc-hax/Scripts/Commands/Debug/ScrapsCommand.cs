namespace Hax;

[DebugCommand("/scraps")]
public class ScrapsCommand : ICommand {
    public void Execute(string[] _) {
        if (!Helper.RoundManager.IsNotNull(out RoundManager roundManager)) {
            Console.Print("RoundManager not found!");
            return;
        }

        roundManager.currentLevel.spawnableScrap.ForEach((i, spawnableScrap) => {
            Logger.Write($"{spawnableScrap.spawnableItem.name} = {i}");
        });
    }
}
