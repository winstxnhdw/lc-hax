using UnityEngine;

[DebugCommand("/enemies")]
internal class EnemiesCommand : ICommand {
    public void Execute(StringArray _) {
        Resources.FindObjectsOfTypeAll<EnemyType>().ForEach(enemy =>
            Logger.Write(enemy.enemyName)
        );
    }
}
