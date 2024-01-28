using Hax;
using System;
using UnityEngine;

[Command("/enemies")]
public class LevelEnemiesCommand : ICommand {

    public void Execute(StringArray _) {
        if (Helper.RoundManager == null) return;
        if (Helper.RoundManager.currentLevel == null) return;
        string Msg = "";
        foreach (System.Collections.Generic.KeyValuePair<string, GameObject> enemy in Helper.SpawnableEnemies) {
            Msg += $"{enemy.Key}, ";
        }
        Console.Write(Msg);
        Chat.Print(Msg);

    }
}
