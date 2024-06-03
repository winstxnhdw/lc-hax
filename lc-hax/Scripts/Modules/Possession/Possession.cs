class Possession {
    EnemyAI? enemy = null;

    internal EnemyAI? Enemy => this.enemy.Unfake();

    internal bool IsPossessed => this.Enemy is not null;

    internal void SetEnemy(EnemyAI enemy) => this.enemy = enemy;

    internal void Clear() => this.enemy = null;
}
