using GameNetcodeStuff;

internal class CoilHeadController : IEnemyController<SpringManAI> {
    bool GetStoppingMovement(SpringManAI enemy) => enemy.Reflect().GetInternalField<bool>("stoppingMovement");

    public void OnSecondarySkillHold(SpringManAI enemy) => enemy.SetAnimationGoServerRpc();

    public void ReleaseSecondarySkill(SpringManAI enemy) => enemy.SetAnimationStopServerRpc();

    public bool IsAbleToMove(SpringManAI enemy) => !this.GetStoppingMovement(enemy);

    public bool IsAbleToRotate(SpringManAI enemy) => !this.GetStoppingMovement(enemy);

    public float InteractRange(SpringManAI _) => 1.5f;

    public void OnOutsideStatusChange(SpringManAI enemy) => enemy.StopSearch(enemy.searchForPlayers, true);

    public void OnCollideWithPlayer(SpringManAI enemy, PlayerControllerB player) => enemy.OnCollideWithPlayer(player.playerCollider);
}

