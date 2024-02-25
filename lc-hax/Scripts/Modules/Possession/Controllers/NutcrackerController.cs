using GameNetcodeStuff;
using Hax;

enum NutcrackerState {
    WALKING,
    SENTRY
}

internal class NutcrackerController : IEnemyController<NutcrackerEnemyAI> {
    public void GetCameraPosition(NutcrackerEnemyAI enemy) {
        PossessionMod.CamOffsetY = 2.8f;
        PossessionMod.CamOffsetZ = -3f;
    }

    bool InSentryMode { get; set; } = false;

    float GetStunNormalizedTimer(NutcrackerEnemyAI enemy) => enemy.Reflect().GetInternalField<float>("stunNormalizedTimer");

    float GetTimeSinceHittingPlayer(NutcrackerEnemyAI enemy) => enemy.Reflect().GetInternalField<float>("timeSinceHittingPlayer");

    void SetTimeSinceHittingPlayer(NutcrackerEnemyAI enemy, float value) => enemy.Reflect().SetInternalField("timeSinceHittingPlayer", value);

    public void Update(NutcrackerEnemyAI enemy, bool isAIControlled) {
        if (isAIControlled) return;
        if (this.InSentryMode) return;
        enemy.SwitchToBehaviourServerRpc(unchecked((int)NutcrackerState.WALKING)); // See #415
    }

    public bool IsAbleToRotate(NutcrackerEnemyAI enemy) => !enemy.IsBehaviourState(NutcrackerState.SENTRY);

    public bool IsAbleToMove(NutcrackerEnemyAI enemy) => !enemy.IsBehaviourState(NutcrackerState.SENTRY);

    public void UsePrimarySkill(NutcrackerEnemyAI enemy) {
        if (enemy.gun is not ShotgunItem shotgun) return;

        shotgun.gunShootAudio.volume = 0.25f;
        enemy.FireGunServerRpc();
    }

    public void OnSecondarySkillHold(NutcrackerEnemyAI enemy) {
        enemy.SetBehaviourState(NutcrackerState.SENTRY);
        this.InSentryMode = true;
    }

    public void ReleaseSecondarySkill(NutcrackerEnemyAI enemy) {
        enemy.SetBehaviourState(NutcrackerState.WALKING);
        this.InSentryMode = false;
    }

    public void OnUnpossess(NutcrackerEnemyAI enemy) => this.InSentryMode = false;

    public string GetPrimarySkillName(NutcrackerEnemyAI enemy) => enemy.gun is null ? "" : "Fire";

    public string GetSecondarySkillName(NutcrackerEnemyAI _) => "(HOLD) Sentry mode";

    public float InteractRange(NutcrackerEnemyAI _) => 1.5f;

    public void OnOutsideStatusChange(NutcrackerEnemyAI enemy) {
        enemy.StopSearch(enemy.attackSearch, true);
        enemy.StopSearch(enemy.patrol, true);
    }

    public void OnCollideWithPlayer(NutcrackerEnemyAI enemy, PlayerControllerB player) {
        if (enemy.isOutside) {
            if (enemy.isEnemyDead) return;
            if (this.GetTimeSinceHittingPlayer(enemy) < 1f) return;
            if (this.GetStunNormalizedTimer(enemy) >= 0f) return;

            this.SetTimeSinceHittingPlayer(enemy, 0f);
            enemy.LegKickPlayerServerRpc((int)player.actualClientId);
        }
    }
}
