class CrawlerController : IEnemyController<CrawlerAI> {
    public bool SyncAnimationSpeedEnabled(CrawlerAI _) => false;


    public void UsePrimarySkill(CrawlerAI enemy) => enemy.MakeScreechNoiseServerRpc();
    public void UseSecondarySkill(CrawlerAI enemy) => enemy.CollideWithWallServerRpc();


    public void OnOutsideStatusChange(CrawlerAI enemy) => enemy.StopSearch(enemy.searchForPlayers, true);

    public string GetPrimarySkillName(CrawlerAI _) => "Make Screech Noise";
    public string GetSecondarySkillName(CrawlerAI _) => "Camera Shake";

}
