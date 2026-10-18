sealed class PossessionControl {
    EnemyAI Enemy { get; }
    IEnemyController Controller { get; }

    PossessionExecution Execution { get; set; }
    internal bool HasManualAuthority => this.Execution is PossessionExecution.Manual && this.Enemy.Unfake() is { IsSpawned: true, IsOwner: true, isEnemyDead: false };
    internal bool IsManuallyControlled => this.HasManualAuthority && !this.Controller.RequiresServerAI(false);

    internal PossessionControl(EnemyAI enemy, IEnemyController controller) {
        this.Enemy = enemy;
        this.Controller = controller;
    }

    internal bool ChangeExecution(PossessionExecution execution) {
        if (this.Execution == execution) return false;

        this.Execution = execution;
        return true;
    }
}
