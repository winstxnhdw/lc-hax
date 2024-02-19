using Hax;

enum NutcrackerState {
    WALKING,
    SENTRY
}

internal class NutcrackerController : IEnemyController<NutcrackerEnemyAI> {
    public void OnMovement(NutcrackerEnemyAI enemy, bool isMoving, bool isSprinting) {
        if (!isMoving && !isSprinting) return;
        enemy.SetBehaviourState(NutcrackerState.WALKING);
    }

    public bool IsAbleToRotate(NutcrackerEnemyAI enemy) => !enemy.IsBehaviourState(NutcrackerState.SENTRY);

    public bool IsAbleToMove(NutcrackerEnemyAI enemy) => !enemy.IsBehaviourState(NutcrackerState.SENTRY);

    public void UsePrimarySkill(NutcrackerEnemyAI enemy) {
        if (enemy.gun is not ShotgunItem shotgun) return;

        shotgun.gunShootAudio.volume = 0.25f;
        enemy.FireGunServerRpc();
    }

    public void OnSecondarySkillHold(NutcrackerEnemyAI enemy) => enemy.SetBehaviourState(NutcrackerState.SENTRY);

    public void ReleaseSecondarySkill(NutcrackerEnemyAI enemy) => enemy.SetBehaviourState(NutcrackerState.WALKING);

    public string GetPrimarySkillName(NutcrackerEnemyAI enemy) => enemy.gun is null ? "" : "Fire";

    public string GetSecondarySkillName(NutcrackerEnemyAI _) => "(HOLD) Sentry mode";

    public float InteractRange(NutcrackerEnemyAI _) => 1.5f;
}
