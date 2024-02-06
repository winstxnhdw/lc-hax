using Hax;

enum JesterState {
    CLOSED,
    CRANKING,
    OPEN
}

public class JesterController : IEnemyController<JesterAI> {
    public void UsePrimarySkill(JesterAI enemyInstance) => enemyInstance.SetBehaviourState(JesterState.CLOSED);

    public void UseSecondarySkill(JesterAI enemyInstance) {
        if (!enemyInstance.IsBehaviourState(JesterState.CLOSED)) return;
        enemyInstance.SetBehaviourState(JesterState.CRANKING);
    }

    public void ReleaseSecondarySkill(JesterAI enemyInstance) {
        if (!enemyInstance.IsBehaviourState(JesterState.CRANKING)) return;
        enemyInstance.SetBehaviourState(JesterState.OPEN);
    }

    public bool IsAbleToMove(JesterAI enemyInstance) => !enemyInstance.IsBehaviourState(JesterState.CRANKING);

    public CharArray GetPrimarySkillName(JesterAI _) => "Close box";

    public CharArray GetSecondarySkillName(JesterAI _) => "(HOLD) Begin cranking";
}
