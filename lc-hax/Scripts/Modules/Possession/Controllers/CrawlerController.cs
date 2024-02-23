using GameNetcodeStuff;

internal class CrawlerController : IEnemyController<CrawlerAI> {

    float GettimeSinceHittingPlayer(CrawlerAI enemy) => enemy.Reflect().GetInternalField<float>("timeSinceHittingPlayer");

    void SettimeSinceHittingPlayer(CrawlerAI enemy, float value) => enemy.Reflect().SetInternalField("timeSinceHittingPlayer", value);

    public float InteractRange(CrawlerAI _) => 1.5f;

    public bool SyncAnimationSpeedEnabled(CrawlerAI _) => false;

    public void UseSecondarySkill(CrawlerAI enemy) => enemy.MakeScreechNoiseServerRpc();

    public void UsePrimarySkill(CrawlerAI enemy) => enemy.CollideWithWallServerRpc();

    public void OnOutsideStatusChange(CrawlerAI enemy) => enemy.StopSearch(enemy.searchForPlayers, true);

    public void OnCollideWithPlayer(CrawlerAI enemy, PlayerControllerB player) {
        if (enemy.isOutside) {
            if (this.GettimeSinceHittingPlayer(enemy) < 0.65f) {
                return;
            }
            this.SettimeSinceHittingPlayer(enemy, 0f);
            player.DamagePlayer(40, true, true, CauseOfDeath.Mauling, 0, false, default);
            enemy.HitPlayerServerRpc((int)GameNetworkManager.Instance.localPlayerController.playerClientId);
        }
    }

}
