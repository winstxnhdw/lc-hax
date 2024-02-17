using Hax;

enum GiantState {
    DEFAULT = 0,
    CHASE = 1
}

internal class ForestGiantController : IEnemyController<ForestGiantAI> {
    bool IsUsingSecondarySkill { get; set; } = false;

    public void OnMovement(ForestGiantAI enemyInstance, bool isMoving, bool isSprinting) {
        if (!this.IsUsingSecondarySkill) {
            enemyInstance.SetBehaviourState(GiantState.DEFAULT);
        }
    }

    public void UseSecondarySkill(ForestGiantAI enemyInstance) {
        this.IsUsingSecondarySkill = true;
        enemyInstance.SetBehaviourState(GiantState.CHASE);
    }

    public void ReleaseSecondarySkill(ForestGiantAI enemyInstance) {
        this.IsUsingSecondarySkill = false;
        enemyInstance.SetBehaviourState(GiantState.DEFAULT);
    }

    public bool IsAbleToMove(ForestGiantAI enemyInstance) => !enemyInstance.Reflect().GetInternalField<bool>("inEatingPlayerAnimation");

    public string GetSecondarySkillName(ForestGiantAI _) => "(HOLD) Chase";

    public bool CanUseEntranceDoors(ForestGiantAI _) => false;

    public float InteractRange(ForestGiantAI _) => 0.0f;

    public void OnUnpossess(ForestGiantAI enemyInstance) => this.IsUsingSecondarySkill = false;

    public bool SyncAnimationSpeedEnabled(ForestGiantAI _) => false;
}
