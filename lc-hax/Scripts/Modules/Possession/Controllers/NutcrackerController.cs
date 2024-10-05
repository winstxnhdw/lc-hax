enum NutcrackerState {
    WALKING,
    SENTRY
}

class NutcrackerController : IEnemyController<NutcrackerEnemyAI> {
    bool InSentryMode { get; set; } = false;

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
}
