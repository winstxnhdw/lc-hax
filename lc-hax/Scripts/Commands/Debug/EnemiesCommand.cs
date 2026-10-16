using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[DebugCommand("enemies")]
sealed class EnemiesCommand : ICommand {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        Resources.FindObjectsOfTypeAll<EnemyType>().ForEach(enemy =>
            Logger.Write(enemy.enemyName)
        );
        return Task.CompletedTask;
    }
}
