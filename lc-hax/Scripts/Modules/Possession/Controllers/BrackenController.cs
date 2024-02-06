using Hax;

enum FlowerMan {
    DEFAULT = 0,
    STAND = 1
}

internal class BrackenController : IEnemyController<FlowermanAI> {
    internal void UsePrimarySkill(FlowermanAI enemyInstance) {
        if (!enemyInstance.carryingPlayerBody) return;

        _ = enemyInstance.Reflect().InvokeInternalMethod("DropPlayerBody");
        enemyInstance.DropPlayerBodyServerRpc();
    }

    internal void UseSecondarySkill(FlowermanAI enemyInstance) => enemyInstance.SetBehaviourState(FlowerMan.STAND);

    internal void ReleaseSecondarySkill(FlowermanAI enemyInstance) => enemyInstance.SetBehaviourState(FlowerMan.DEFAULT);

    internal bool IsAbleToMove(FlowermanAI enemyInstance) => !enemyInstance.inSpecialAnimation;

    internal string GetPrimarySkillName(FlowermanAI enemyInstance) => enemyInstance.carryingPlayerBody ? "Drop body" : "";

    internal string GetSecondarySkillName(FlowermanAI _) => "Stand";
}
