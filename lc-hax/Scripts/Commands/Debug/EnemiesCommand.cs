using Hax;
using System;
using System.Collections.Generic;
using System.Linq;

[DebugCommand("enemies")]
internal class EnemiesCommand : ICommand {

    static bool IsHostileEnemy(EnemyType enemy) =>
        !enemy.enemyName.Contains("Docile Locust Bees", StringComparison.OrdinalIgnoreCase) ||
        !enemy.enemyName.Contains("Manticoil", StringComparison.OrdinalIgnoreCase);

    static List<string> HostileEnemies { get; } =
        Resources.FindObjectsOfTypeAll<EnemyType>()
            .Where(EnemiesCommand.IsHostileEnemy)
            .Select(enemy => enemy.enemyName)
            .ToList();

    public void Execute(StringArray _) {
        EnemyType[] enemies = Resources.FindObjectsOfTypeAll<EnemyType>();
        string enemy = string.Join(", ", enemies.Select(e => e.enemyName));
        Helper.SendNotification("Available Enemies", enemy, false);
        Logger.Write(enemy);
    }
}
