using UnityEngine;

[DebugCommand("enemies")]
class EnemiesCommand : ICommand {
    public void Execute(StringArray _) {
        Resources.FindObjectsOfTypeAll<EnemyType>().ForEach(enemy =>
            Logger.Write(enemy.enemyName)
        );
    }
}
