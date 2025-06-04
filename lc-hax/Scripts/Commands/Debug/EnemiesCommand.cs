using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[DebugCommand("enemies")]
class EnemiesCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        Resources.FindObjectsOfTypeAll<EnemyType>().ForEach(enemy =>
            Logger.Write(enemy.enemyName)
        );
    }
}
