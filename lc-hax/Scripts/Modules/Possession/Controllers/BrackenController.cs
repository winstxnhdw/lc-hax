using Hax;

enum FlowerMan {
    DEFAULT = 0,
    STAND = 1
}

public class BrackenController : IEnemyController<FlowermanAI> {
    public void UsePrimarySkill(FlowermanAI enemyInstance) {
        if (!enemyInstance.carryingPlayerBody) return;

        _ = enemyInstance.Reflect().InvokeInternalMethod("DropPlayerBody");
        enemyInstance.DropPlayerBodyServerRpc();
    }

    public void UseSecondarySkill(FlowermanAI enemyInstance) => enemyInstance.SetBehaviourState(FlowerMan.STAND);

    public void ReleaseSecondarySkill(FlowermanAI enemyInstance) => enemyInstance.SetBehaviourState(FlowerMan.DEFAULT);

    public bool IsAbleToMove(FlowermanAI enemyInstance) => !enemyInstance.inSpecialAnimation;

    public string GetPrimarySkillName(FlowermanAI enemyInstance) => enemyInstance.carryingPlayerBody ? "Drop body" : "";

    public string GetSecondarySkillName(FlowermanAI _) => "Stand";
}
