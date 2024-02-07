using Hax;

enum NutCrackerState {
    WALKING,
    SENTRY
}

internal class NutcrackerController : IEnemyController<NutcrackerEnemyAI> {

    private bool isSecondarySkillActive = false;

    public void OnMovement(NutcrackerEnemyAI enemyInstance, bool isMoving, bool isSprinting) {
        if (isMoving || isSprinting) {
            enemyInstance.SetBehaviourState(NutCrackerState.WALKING);
        }
    }

    public void UsePrimarySkill(NutcrackerEnemyAI enemyInstance) {
        if (enemyInstance.gun is not ShotgunItem shotgun) return;

        shotgun.gunShootAudio.volume = 0.25f;
        enemyInstance.FireGunServerRpc();
    }

    public void UseSecondarySkill(NutcrackerEnemyAI enemyInstance) {
        if (!this.isSecondarySkillActive) {
            enemyInstance.SetBehaviourState(NutCrackerState.SENTRY);
            this.isSecondarySkillActive = true;
        }
    }



    public void ReleaseSecondarySkill(NutcrackerEnemyAI enemyInstance) {
        if (this.isSecondarySkillActive) {
            enemyInstance.SetBehaviourState(NutCrackerState.WALKING);
            this.isSecondarySkillActive = false;
        }
    }

    public string GetPrimarySkillName(NutcrackerEnemyAI enemyInstance) => enemyInstance.gun is null ? "" : "Fire";

    public string GetSecondarySkillName(NutcrackerEnemyAI _) => "(HOLD) Sentry mode";
}
