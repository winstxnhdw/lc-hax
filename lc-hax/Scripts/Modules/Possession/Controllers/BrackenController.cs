enum BrackenState {
    SCOUTING,
    STAND,
    ANGER
}

sealed class BrackenController : IEnemyController<FlowermanAI> {
    public void UsePrimarySkill(FlowermanAI enemy) {
        if (!enemy.carryingPlayerBody) {
            enemy.SetBehaviourState(BrackenState.ANGER);
        }

        enemy.DropPlayerBodyServerRpc();
    }

    public void UseSecondarySkill(FlowermanAI enemy) => enemy.SetBehaviourState(BrackenState.STAND);

    public void ReleaseSecondarySkill(FlowermanAI enemy) => enemy.SetBehaviourState(BrackenState.SCOUTING);

    public bool IsAbleToMove(FlowermanAI enemy) => !enemy.inSpecialAnimation;

    public string GetPrimarySkillName(FlowermanAI enemy) => enemy.carryingPlayerBody ? "Drop body" : "";

    public string GetSecondarySkillName(FlowermanAI _) => "Stand";

    public float InteractRange(FlowermanAI _) => 1.5f;

    public bool SyncAnimationSpeedEnabled(FlowermanAI _) => false;
}
