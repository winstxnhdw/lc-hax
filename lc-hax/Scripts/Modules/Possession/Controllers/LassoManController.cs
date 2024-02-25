internal class LassoManController : IEnemyController<LassoManAI> {
    public void GetCameraPosition(LassoManAI enemy) {
        PossessionMod.CamOffsetY = 2.8f;
        PossessionMod.CamOffsetZ = -3.5f;
    }

    public void UsePrimarySkill(LassoManAI enemy) => enemy.MakeScreechNoiseServerRpc();

    public bool SyncAnimationSpeedEnabled(LassoManAI enemy) => false;

    public void OnOutsideStatusChange(LassoManAI enemy) => enemy.StopSearch(enemy.searchForPlayers, true);

}

