internal class Possession
{
    private EnemyAI? enemy = null;

    internal EnemyAI? Enemy => enemy.Unfake();

    internal bool IsPossessed => Enemy is not null;

    internal void SetEnemy(EnemyAI enemy)
    {
        this.enemy = enemy;
    }

    internal void Clear()
    {
        enemy = null;
    }
}