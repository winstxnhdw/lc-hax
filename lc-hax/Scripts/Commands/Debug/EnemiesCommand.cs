using Hax;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DebugCommand("enemies")]
internal class EnemiesCommand : ICommand {

    static bool IsHostileEnemy(EnemyType enemy) =>
        !enemy.enemyName.Contains("Docile Locust Bees", StringComparison.OrdinalIgnoreCase) ||
        !enemy.enemyName.Contains("Manticoil", StringComparison.OrdinalIgnoreCase);

    string[] HostileEnemies { get; } =
        Resources.FindObjectsOfTypeAll<EnemyType>()
            .Where(EnemiesCommand.IsHostileEnemy)
            .Select(enemy => enemy.enemyName)
            .ToArray();

    public void Execute(StringArray _) {
        string enemy = string.Join(", ", HostileEnemies);
        Helper.SendNotification("Available Enemies", enemy, false);
        Logger.Write(enemy);
    }
}
