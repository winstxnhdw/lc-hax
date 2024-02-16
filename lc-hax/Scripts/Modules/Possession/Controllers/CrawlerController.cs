using Hax;


internal class CrawlerController : IEnemyController<CrawlerAI> {
    public float? InteractRange(CrawlerAI _) => 1.5f;

    public bool SyncAnimationSpeedEnabled(CrawlerAI _) => false;
}
