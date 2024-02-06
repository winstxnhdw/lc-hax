using Hax;

enum JesterState {
    CLOSED,
    CRANKING,
    OPEN
}

internal class JesterController : IEnemyController<JesterAI> {
    internal void UsePrimarySkill(JesterAI enemyInstance) => enemyInstance.SetBehaviourState(JesterState.CLOSED);

    internal void UseSecondarySkill(JesterAI enemyInstance) {
        if (!enemyInstance.IsBehaviourState(JesterState.CLOSED)) return;
        enemyInstance.SetBehaviourState(JesterState.CRANKING);
    }

    internal void ReleaseSecondarySkill(JesterAI enemyInstance) {
        if (!enemyInstance.IsBehaviourState(JesterState.CRANKING)) return;
        enemyInstance.SetBehaviourState(JesterState.OPEN);
    }

    internal bool IsAbleToMove(JesterAI enemyInstance) => !enemyInstance.IsBehaviourState(JesterState.CRANKING);

    internal CharArray GetPrimarySkillName(JesterAI _) => "Close box";

    internal CharArray GetSecondarySkillName(JesterAI _) => "(HOLD) Begin cranking";
}
