internal class SpringManEnemyController : IEnemyController<SpringManAI> {
    bool HoldingForceMove { get; set; } = false;

    bool GetHasStopped(SpringManAI enemyInstance) => enemyInstance.Reflect().GetInternalField<bool>("hasStopped");

    bool GetStoppingMovement(SpringManAI enemyInstance) => enemyInstance.Reflect().GetInternalField<bool>("stoppingMovement");

    void SetHasStopped(SpringManAI enemyInstance, bool value) => enemyInstance.Reflect().SetInternalField("hasStopped", value);

    void SetStoppingMovement(SpringManAI enemyInstance, bool value) => enemyInstance.Reflect().SetInternalField("stoppingMovement", value);

    public void Update(SpringManAI enemyInstance) {
        if (!this.HoldingForceMove) return;

        this.SetHasStopped(enemyInstance, false);
        this.SetStoppingMovement(enemyInstance, false);
    }

    public void UseSecondarySkill(SpringManAI enemyInstance) => this.HoldingForceMove = true;

    public void ReleaseSecondarySkill(SpringManAI enemyInstance) => this.HoldingForceMove = false;

    public bool IsAbleToMove(SpringManAI enemyInstance) =>
        this.HoldingForceMove || !this.GetHasStopped(enemyInstance) || !this.GetStoppingMovement(enemyInstance);

    public bool IsAbleToRotate(SpringManAI enemyInstance) =>
        this.HoldingForceMove || !this.GetHasStopped(enemyInstance) || !this.GetStoppingMovement(enemyInstance);

    public float? InteractRange(SpringManAI _) => 1.5f;
}

