sealed class Possession() {
    EnemyAI? EnemyReference { get; set; }

    internal EnemyAI? Enemy => this.EnemyReference.Unfake();

    internal bool IsPossessed => this.Enemy is not null;

    internal void SetEnemy(EnemyAI enemy) => this.EnemyReference = enemy;

    internal void Clear() => this.EnemyReference = null;
}
