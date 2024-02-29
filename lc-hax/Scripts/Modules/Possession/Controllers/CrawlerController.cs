internal class CrawlerController : IEnemyController<CrawlerAI> {

    float GettimeSinceHittingPlayer(CrawlerAI enemy) => enemy.Reflect().GetInternalField<float>("timeSinceHittingPlayer");

    void SettimeSinceHittingPlayer(CrawlerAI enemy, float value) => enemy.Reflect().SetInternalField("timeSinceHittingPlayer", value);

    public bool SyncAnimationSpeedEnabled(CrawlerAI _) => false;

    public void UseSecondarySkill(CrawlerAI enemy) => enemy.MakeScreechNoiseServerRpc();

    public void UsePrimarySkill(CrawlerAI enemy) => enemy.CollideWithWallServerRpc();

    public void OnOutsideStatusChange(CrawlerAI enemy) => enemy.StopSearch(enemy.searchForPlayers, true);

}
