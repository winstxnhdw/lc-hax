internal class CrawlerController : IEnemyController<CrawlerAI> {

    public bool SyncAnimationSpeedEnabled(CrawlerAI _) => false;

    public void UseSecondarySkill(CrawlerAI enemy) => enemy.MakeScreechNoiseServerRpc();

    public void UsePrimarySkill(CrawlerAI enemy) => enemy.CollideWithWallServerRpc();

    public void OnOutsideStatusChange(CrawlerAI enemy) => enemy.StopSearch(enemy.searchForPlayers, true);

}
