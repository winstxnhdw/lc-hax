using Hax;

enum GiantState {
    DEFAULT = 0,
    CHASE = 1
}

internal class ForestGiantController : IEnemyController<ForestGiantAI> {

    bool isUsingSecondarySkill = false;

    public void OnMovement(ForestGiantAI enemyInstance, bool isMoving, bool isSprinting) {
        if (!this.isUsingSecondarySkill) {
            enemyInstance.SetBehaviourState(GiantState.DEFAULT);
        }
    }

    public void UseSecondarySkill(ForestGiantAI enemyInstance) {
        this.isUsingSecondarySkill = true;
        enemyInstance.SetBehaviourState(GiantState.CHASE);
    }


    public void ReleaseSecondarySkill(ForestGiantAI enemyInstance) {
        this.isUsingSecondarySkill = false;
        enemyInstance.SetBehaviourState(GiantState.DEFAULT);
    }

    public bool IsAbleToMove(ForestGiantAI enemyInstance) => !enemyInstance.Reflect().GetInternalField<bool>("inEatingPlayerAnimation");

    public string GetSecondarySkillName(ForestGiantAI _) => "(HOLD) Chase";

    public bool CanUseEntranceDoors(ForestGiantAI _) => false;

    public float? InteractRange(ForestGiantAI _) => 0f;

    public void OnUnpossess(ForestGiantAI enemyInstance) => this.isUsingSecondarySkill = false;

    public bool SyncAnimationSpeedEnabled(ForestGiantAI _) => false;
}
