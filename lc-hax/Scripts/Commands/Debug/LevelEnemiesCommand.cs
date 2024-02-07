using Hax;
using System;
using UnityEngine;

[DebugCommand("/enemies")]
public class LevelEnemiesCommand : ICommand {

    public void Execute(StringArray _) {
        if (Helper.RoundManager == null || Helper.RoundManager.currentLevel == null) return;

        // Using string.Join to concatenate enemy names
        string levelEnemies = string.Join(", ", Helper.SpawnableEnemies.Keys);

        Console.Write(levelEnemies);
        Chat.Print(levelEnemies);
    }
}
