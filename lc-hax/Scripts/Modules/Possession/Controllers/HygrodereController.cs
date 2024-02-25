using GameNetcodeStuff;

internal class HygrodereController : IEnemyController<BlobAI> {
    public void GetCameraPosition(BlobAI enemy) {
        PossessionMod.CamOffsetY = 2f;
        PossessionMod.CamOffsetZ = -3f;
    }

    void SetTamedTimer(BlobAI enemy, float time) => enemy.Reflect().SetInternalField("tamedTimer", time);

    void SetAngeredTimer(BlobAI enemy, float time) => enemy.Reflect().SetInternalField("angeredTimer", time);

    public void OnSecondarySkillHold(BlobAI enemy) {
        this.SetAngeredTimer(enemy, 0.0f);
        this.SetTamedTimer(enemy, 2.0f);
    }

    public void ReleaseSecondarySkill(BlobAI enemy) => this.SetTamedTimer(enemy, 0.0f);

    public void UsePrimarySkill(BlobAI enemy) => this.SetAngeredTimer(enemy, 18.0f);

    public float InteractRange(BlobAI _) => 3.5f;

    public float SprintMultiplier(BlobAI _) => 9.8f;

    public void OnOutsideStatusChange(BlobAI enemy) => enemy.StopSearch(enemy.searchForPlayers, true);

    public void OnCollideWithPlayer(BlobAI enemy, PlayerControllerB player) => enemy.OnCollideWithPlayer(player.playerCollider);
}

