using Hax;

enum GiantState {
    DEFAULT = 0,
    CHASE = 1
}

internal class ForestGiantController : IEnemyController<ForestGiantAI> {
    public void UseSecondarySkill(ForestGiantAI enemyInstance) => enemyInstance.SetBehaviourState(GiantState.CHASE);

    public void ReleaseSecondarySkill(ForestGiantAI enemyInstance) => enemyInstance.SetBehaviourState(GiantState.DEFAULT);

    public bool IsAbleToMove(ForestGiantAI enemyInstance) => !enemyInstance.Reflect().GetInternalField<bool>("inEatingPlayerAnimation");

    public string GetSecondarySkillName(ForestGiantAI _) => "(HOLD) Chase";

    public bool CanUseEntranceDoors(ForestGiantAI _) => false;

    public float? InteractRange(ForestGiantAI _) => 0f;
}
