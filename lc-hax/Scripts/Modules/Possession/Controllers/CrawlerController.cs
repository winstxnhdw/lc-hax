internal class CrawlerController : IEnemyController<CrawlerAI> {
    public float InteractRange(CrawlerAI _) => 1.5f;

    public bool SyncAnimationSpeedEnabled(CrawlerAI _) => false;

    public void UseSecondarySkill(CrawlerAI enemy) => enemy.MakeScreechNoiseServerRpc();

    public void UsePrimarySkill(CrawlerAI enemy) => enemy.CollideWithWallServerRpc();

    public void OnOutsideStatusChange(CrawlerAI enemy) => enemy.StopSearch(enemy.searchForPlayers, true);

}
