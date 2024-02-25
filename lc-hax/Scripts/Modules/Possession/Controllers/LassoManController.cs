class LassoManController : IEnemyController<LassoManAI> {
    public void UsePrimarySkill(LassoManAI enemy) => enemy.MakeScreechNoiseServerRpc();

    public bool SyncAnimationSpeedEnabled(LassoManAI enemy) => false;

    public void OnOutsideStatusChange(LassoManAI enemy) => enemy.StopSearch(enemy.searchForPlayers, true);

}

