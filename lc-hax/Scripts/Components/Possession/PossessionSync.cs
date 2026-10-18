using UnityEngine;

sealed class PossessionSync(EnemyAI enemy) {
    const float PositionSyncInterval = 0.05f;
    const float MinimumRotationSyncAngle = 1.0f;

    EnemyAI EnemyReference { get; } = enemy;
    float CooldownRemaining { get; set; }

    internal void Tick() => this.CooldownRemaining -= Time.deltaTime;

    internal void Synchronize(bool force = false) {
        if (this.EnemyReference.Unfake() is not { IsSpawned: true, IsOwner: true } enemy) return;
        if (!force && this.CooldownRemaining > 0.0f) return;

        enemy.SyncPositionToClients();
        this.CooldownRemaining = PossessionSync.PositionSyncInterval;
        float rotationDifference = Mathf.Abs(Mathf.DeltaAngle(enemy.targetYRotation, enemy.transform.eulerAngles.y));

        if (force || rotationDifference >= PossessionSync.MinimumRotationSyncAngle) {
            enemy.previousYRotation = enemy.transform.eulerAngles.y;
            enemy.targetYRotation = enemy.previousYRotation;
            enemy.UpdateEnemyRotationServerRpc((short)enemy.transform.eulerAngles.y);
        }
    }
}
