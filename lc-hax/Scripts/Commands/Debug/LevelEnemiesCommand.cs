using Hax;

[DebugCommand("/enemies")]
public class LevelEnemiesCommand : ICommand {

    public void Execute(StringArray _) {
        if (Helper.RoundManager == null || Helper.RoundManager.currentLevel == null) return;

        string levelEnemies = string.Join(", ", Helper.AllSpawnableEnemies.Keys);

        Logger.Write(levelEnemies);
        Chat.Print(levelEnemies);
    }
}
