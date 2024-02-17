using Hax;

enum NutCrackerState {
    WALKING,
    SENTRY
}

internal class NutcrackerController : IEnemyController<NutcrackerEnemyAI> {
    bool IsSecondarySkillActive { get; set; } = false;

    public void OnMovement(NutcrackerEnemyAI enemy, bool isMoving, bool isSprinting) {
        if (!isMoving && !isSprinting) return;
        enemy.SetBehaviourState(NutCrackerState.WALKING);
    }

    public void UsePrimarySkill(NutcrackerEnemyAI enemy) {
        if (enemy.gun is not ShotgunItem shotgun) return;

        shotgun.gunShootAudio.volume = 0.25f;
        enemy.FireGunServerRpc();
    }

    public void UseSecondarySkill(NutcrackerEnemyAI enemy) {
        if (this.IsSecondarySkillActive) return;

        enemy.SetBehaviourState(NutCrackerState.SENTRY);
        this.IsSecondarySkillActive = true;
    }

    public void ReleaseSecondarySkill(NutcrackerEnemyAI enemy) {
        enemy.SetBehaviourState(NutCrackerState.WALKING);
        this.IsSecondarySkillActive = false;
    }

    public string GetPrimarySkillName(NutcrackerEnemyAI enemy) => enemy.gun is null ? "" : "Fire";

    public string GetSecondarySkillName(NutcrackerEnemyAI _) => "(HOLD) Sentry mode";

    public float InteractRange(NutcrackerEnemyAI _) => 1.5f;
}
