using Hax;

enum FlowerMan {
    SCOUTING = 0,
    STAND = 1,
    ANGER = 2,
}

internal class BrackenController : IEnemyController<FlowermanAI> {
    internal void UsePrimarySkill(FlowermanAI enemyInstance) {
        if (!enemyInstance.carryingPlayerBody) {
            enemyInstance.SetBehaviourState(FlowerMan.ANGER);
        }

        enemyInstance.DropPlayerBodyServerRpc();
    }

    internal void UseSecondarySkill(FlowermanAI enemyInstance) => enemyInstance.SetBehaviourState(FlowerMan.STAND);

    internal void ReleaseSecondarySkill(FlowermanAI enemyInstance) => enemyInstance.SetBehaviourState(FlowerMan.SCOUTING);

    internal bool IsAbleToMove(FlowermanAI enemyInstance) => !enemyInstance.inSpecialAnimation;

    internal string GetPrimarySkillName(FlowermanAI enemyInstance) => enemyInstance.carryingPlayerBody ? "Drop body" : "";

    internal string GetSecondarySkillName(FlowermanAI _) => "Stand";
}
