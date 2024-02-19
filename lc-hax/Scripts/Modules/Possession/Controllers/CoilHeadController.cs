internal class CoilHeadEnemyController : IEnemyController<SpringManAI> {
    bool HoldingForceMove { get; set; } = false;

    bool GetHasStopped(SpringManAI enemy) => enemy.Reflect().GetInternalField<bool>("hasStopped");

    bool GetStoppingMovement(SpringManAI enemy) => enemy.Reflect().GetInternalField<bool>("stoppingMovement");

    void SetHasStopped(SpringManAI enemy, bool value) => enemy.Reflect().SetInternalField("hasStopped", value);

    void SetStoppingMovement(SpringManAI enemy, bool value) => enemy.Reflect().SetInternalField("stoppingMovement", value);

    public void Update(SpringManAI enemy) {
        if (!this.HoldingForceMove) return;

        this.SetHasStopped(enemy, false);
        this.SetStoppingMovement(enemy, false);
    }

    public void OnSecondarySkillHold(SpringManAI enemy) => this.HoldingForceMove = true;

    public void ReleaseSecondarySkill(SpringManAI enemy) => this.HoldingForceMove = false;

    public bool IsAbleToMove(SpringManAI enemy) =>
        this.HoldingForceMove || !this.GetHasStopped(enemy) || !this.GetStoppingMovement(enemy);

    public bool IsAbleToRotate(SpringManAI enemy) =>
        this.HoldingForceMove || !this.GetHasStopped(enemy) || !this.GetStoppingMovement(enemy);

    public float? InteractRange(SpringManAI _) => 1.5f;
}

