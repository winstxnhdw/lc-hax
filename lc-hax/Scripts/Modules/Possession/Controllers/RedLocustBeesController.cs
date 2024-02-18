using Hax;

enum Bees {
    IDLE = 0,
    DEFENSIVE = 1,
    ATTACK = 2
}


internal class RedLocustBeesController : IEnemyController<RedLocustBees> {
    public bool CanUseEntranceDoors(RedLocustBees _) => true;

    public float InteractRange(RedLocustBees _) => 2.5f;

    public void UsePrimarySkill(RedLocustBees enemy) {
        enemy.SetBehaviourState(Bees.ATTACK);
        enemy.EnterAttackZapModeServerRpc(-1);
    }


    public void UseSecondarySkill(RedLocustBees enemy) => enemy.SetBehaviourState(Bees.IDLE);
}

