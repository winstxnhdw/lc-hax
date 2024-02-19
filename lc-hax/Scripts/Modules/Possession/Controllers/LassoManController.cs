internal class LassoManController : IEnemyController<LassoManAI> {
    public void UsePrimarySkill(LassoManAI enemy) => enemy.MakeScreechNoiseServerRpc();

    public bool SyncAnimationSpeedEnabled(LassoManAI enemy) => false;
}

