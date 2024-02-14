using Hax;

enum FlowerMan {
    SCOUTING = 0,
    STAND = 1,
    ANGER = 2,
}

internal class BrackenController : IEnemyController<FlowermanAI> {
    public void UsePrimarySkill(FlowermanAI enemyInstance) {
        if (!enemyInstance.carryingPlayerBody) {
            enemyInstance.SetBehaviourState(FlowerMan.ANGER);
        }

        enemyInstance.DropPlayerBodyServerRpc();
    }

    public void UseSecondarySkill(FlowermanAI enemyInstance) => enemyInstance.SetBehaviourState(FlowerMan.STAND);

    public void ReleaseSecondarySkill(FlowermanAI enemyInstance) => enemyInstance.SetBehaviourState(FlowerMan.SCOUTING);

    public bool IsAbleToMove(FlowermanAI enemyInstance) => !enemyInstance.inSpecialAnimation;

    public string GetPrimarySkillName(FlowermanAI enemyInstance) => enemyInstance.carryingPlayerBody ? "Drop body" : "";

    public string GetSecondarySkillName(FlowermanAI _) => "Stand";

    public bool CanUseEntranceDoors(FlowermanAI _) => true;

}
