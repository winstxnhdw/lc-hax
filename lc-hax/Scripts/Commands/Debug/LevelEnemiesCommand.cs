using Hax;
using System;

[DebugCommand("/enemies")]
public class LevelEnemiesCommand : ICommand {

    public void Execute(StringArray _) {
        if (Helper.RoundManager == null || Helper.RoundManager.currentLevel == null) return;

        // Using string.Join to concatenate enemy names
        string levelEnemies = string.Join(", ", Helper.AllSpawnableEnemies.Keys);

        Console.Write(levelEnemies);
        Chat.Print(levelEnemies);
    }
}
