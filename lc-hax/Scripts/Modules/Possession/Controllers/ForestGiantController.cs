using Hax;

enum GiantState {
    DEFAULT = 0,
    CHASE = 1
}

internal class ForestGiantController : IEnemyController<ForestGiantAI> {
    internal void UseSecondarySkill(ForestGiantAI enemyInstance) => enemyInstance.SetBehaviourState(GiantState.CHASE);

    internal void ReleaseSecondarySkill(ForestGiantAI enemyInstance) => enemyInstance.SetBehaviourState(GiantState.DEFAULT);

    internal bool IsAbleToMove(ForestGiantAI enemyInstance) => enemyInstance.Reflect().GetInternalField<bool>("inEatingPlayerAnimation");

    internal string GetSecondarySkillName(ForestGiantAI _) => "(HOLD) Chase";
}
