using System.Collections.Generic;

namespace Hax;

public static partial class Helper {
    public static HashSet<EnemyAI> Enemies { get; } = [];

    public static T? GetEnemy<T>() where T : EnemyAI =>
        Enemies.First(enemy => enemy is T) is T enemy ? enemy : null;
}
