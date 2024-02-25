using GameNetcodeStuff;

internal class CrawlerController : IEnemyController<CrawlerAI> {
    public void GetCameraPosition(CrawlerAI enemy) {
        PossessionMod.CamOffsetY = 2.5f;
        PossessionMod.CamOffsetZ = -3f;
    }

    float GettimeSinceHittingPlayer(CrawlerAI enemy) => enemy.Reflect().GetInternalField<float>("timeSinceHittingPlayer");

    void SettimeSinceHittingPlayer(CrawlerAI enemy, float value) => enemy.Reflect().SetInternalField("timeSinceHittingPlayer", value);

    public float InteractRange(CrawlerAI _) => 1.5f;

    public bool SyncAnimationSpeedEnabled(CrawlerAI _) => false;

    public void UseSecondarySkill(CrawlerAI enemy) => enemy.MakeScreechNoiseServerRpc();

    public void UsePrimarySkill(CrawlerAI enemy) => enemy.CollideWithWallServerRpc();

    public void OnOutsideStatusChange(CrawlerAI enemy) => enemy.StopSearch(enemy.searchForPlayers, true);

}
