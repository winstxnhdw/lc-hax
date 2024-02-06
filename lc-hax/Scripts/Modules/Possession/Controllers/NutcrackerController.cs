using Hax;

enum NutCrackerState {
    WALKING,
    SENTRY
}

internal class NutcrackerController : IEnemyController<NutcrackerEnemyAI> {
    internal void OnMovement(NutcrackerEnemyAI enemyInstance, bool isMoving, bool isSprinting) => enemyInstance.SetBehaviourState(NutCrackerState.WALKING);

    internal void UsePrimarySkill(NutcrackerEnemyAI enemyInstance) {
        if (enemyInstance.gun is not ShotgunItem shotgun) return;

        shotgun.gunShootAudio.volume = 0.25f;
        enemyInstance.FireGunServerRpc();
    }

    internal void UseSecondarySkill(NutcrackerEnemyAI enemyInstance) => enemyInstance.SetBehaviourState(NutCrackerState.SENTRY);

    internal void ReleaseSecondarySkill(NutcrackerEnemyAI enemyInstance) => enemyInstance.SetBehaviourState(NutCrackerState.WALKING);

    internal CharArray GetPrimarySkillName(NutcrackerEnemyAI enemyInstance) => enemyInstance.gun is null ? "" : "Fire";

    internal CharArray GetSecondarySkillName(NutcrackerEnemyAI _) => "(HOLD) Sentry mode";
}
