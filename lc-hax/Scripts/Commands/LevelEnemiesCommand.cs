using Hax;
using System;
using UnityEngine;

[DebugCommand("/enemies")]
public class LevelEnemiesCommand : ICommand {

    public void Execute(StringArray _) {
        if (Helper.RoundManager == null) return;
        if (Helper.RoundManager.currentLevel == null) return;
        foreach (System.Collections.Generic.KeyValuePair<string, GameObject> enemy in Helper.SpawnableEnemies) {
            string Msg = $"{enemy.Key}";
            Console.Write(Msg);
            Chat.Print(Msg);
        }
    }
}
