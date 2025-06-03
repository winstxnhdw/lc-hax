class CoilHeadController : IEnemyController<SpringManAI> {
    static bool GetStoppingMovement(SpringManAI enemy) => enemy.stoppingMovement;

    public void OnSecondarySkillHold(SpringManAI enemy) => enemy.SetAnimationGoServerRpc();

    public void ReleaseSecondarySkill(SpringManAI enemy) => enemy.SetAnimationStopServerRpc();

    public bool IsAbleToMove(SpringManAI enemy) => !CoilHeadController.GetStoppingMovement(enemy);

    public bool IsAbleToRotate(SpringManAI enemy) => !CoilHeadController.GetStoppingMovement(enemy);

    public float InteractRange(SpringManAI _) => 1.5f;
}

