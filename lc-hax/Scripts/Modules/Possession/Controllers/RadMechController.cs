public enum OldBirdState {
    IDLE = 0,
    ATTACKING = 1
}
class RadMechEnemyController : IEnemyController<RadMechAI> {

    public void Update(RadMechAI enemy, bool isAIControlled) {
        if (enemy.inFlyingMode || enemy.attemptingGrab) {
            IsFiring = false;
        }
        if (enemy.aimingGun != IsFiring) {
            enemy.SetAimingGun(IsFiring);
        }
    }
    public void OnSecondarySkillHold(RadMechAI enemy) {
        if (enemy.inFlyingMode) {
            return;
        }
        if (enemy.focusedThreatTransform == null) {
            enemy.CheckSightForThreat();
        }
        if (enemy.focusedThreatTransform != null) {
            enemy.SetBehaviourState(OldBirdState.ATTACKING);
            IsFiring = true;
        }
    }
    public void ReleaseSecondarySkill(RadMechAI enemy) {
        if (enemy.inFlyingMode) {
            return;
        }
        IsFiring = false;
        enemy.shootTimer = 0f;
        enemy.SetBehaviourState(OldBirdState.IDLE);
    }
    bool IsFiring = false;
}
