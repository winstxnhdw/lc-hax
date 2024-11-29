class Possession() {
    internal EnemyAI? Enemy { get => field.Unfake(); private set; } = null;

    internal bool IsPossessed => this.Enemy is not null;

    internal void SetEnemy(EnemyAI enemy) => this.Enemy = enemy;

    internal void Clear() => this.Enemy = null;
}
