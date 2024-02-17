using Hax;

enum GiantState {
    DEFAULT = 0,
    CHASE = 1
}

internal class ForestGiantController : IEnemyController<ForestGiantAI> {
    bool IsUsingSecondarySkill { get; set; } = false;

    public void OnMovement(ForestGiantAI enemy, bool isMoving, bool isSprinting) {
        if (!this.IsUsingSecondarySkill) {
            enemy.SetBehaviourState(GiantState.DEFAULT);
        }
    }

    public void UseSecondarySkill(ForestGiantAI enemy) {
        this.IsUsingSecondarySkill = true;
        enemy.SetBehaviourState(GiantState.CHASE);
    }

    public void ReleaseSecondarySkill(ForestGiantAI enemy) {
        this.IsUsingSecondarySkill = false;
        enemy.SetBehaviourState(GiantState.DEFAULT);
    }

    public bool IsAbleToMove(ForestGiantAI enemy) => !enemy.Reflect().GetInternalField<bool>("inEatingPlayerAnimation");

    public string GetSecondarySkillName(ForestGiantAI _) => "(HOLD) Chase";

    public bool CanUseEntranceDoors(ForestGiantAI _) => false;

    public float InteractRange(ForestGiantAI _) => 0.0f;

    public void OnUnpossess(ForestGiantAI enemy) => this.IsUsingSecondarySkill = false;

    public bool SyncAnimationSpeedEnabled(ForestGiantAI _) => false;
}
