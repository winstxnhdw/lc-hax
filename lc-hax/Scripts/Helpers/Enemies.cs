using System.Collections.Generic;

namespace Hax;

public static partial class Helper
{
    public static HashSet<EnemyAI> Enemies => EnemyDependencyPatch.ActiveEnemies;

    public static T? GetEnemy<T>() where T : EnemyAI =>
        Helper.Enemies.First(enemy => enemy is T) is T enemy ? enemy : null;
}
