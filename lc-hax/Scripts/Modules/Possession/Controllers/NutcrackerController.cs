using Hax;

enum NutCrackerState {
    WALKING,
    SENTRY
}

internal class NutcrackerController : IEnemyController<NutcrackerEnemyAI> {
    public void OnMovement(NutcrackerEnemyAI enemyInstance, bool isMoving, bool isSprinting) => enemyInstance.SetBehaviourState(NutCrackerState.WALKING);

    public void UsePrimarySkill(NutcrackerEnemyAI enemyInstance) {
        if (enemyInstance.gun is not ShotgunItem shotgun) return;

        shotgun.gunShootAudio.volume = 0.25f;
        enemyInstance.FireGunServerRpc();
    }

    public void UseSecondarySkill(NutcrackerEnemyAI enemyInstance) => enemyInstance.SetBehaviourState(NutCrackerState.SENTRY);

    public void ReleaseSecondarySkill(NutcrackerEnemyAI enemyInstance) => enemyInstance.SetBehaviourState(NutCrackerState.WALKING);

    public string GetPrimarySkillName(NutcrackerEnemyAI enemyInstance) => enemyInstance.gun is null ? "" : "Fire";

    public string GetSecondarySkillName(NutcrackerEnemyAI _) => "(HOLD) Sentry mode";
}
