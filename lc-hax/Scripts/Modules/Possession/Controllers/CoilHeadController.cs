using Hax;
using UnityEngine;

enum CoilHeadState {
    Idle = 0,
    Chase = 1
}

internal class CoilHeadController : IEnemyController<SpringManAI> {

    Vector3 CamOffset { get; } = new(0, 2.8f, -3.5f);

    public Vector3 GetCameraOffset(SpringManAI _) => this.CamOffset;

    bool GetStoppingMovement(SpringManAI enemy) => enemy.Reflect().GetInternalField<bool>("stoppingMovement");

    float GetTimeSinceHittingPlayer(SpringManAI enemy) =>
        enemy.Reflect().GetInternalField<float>("timeSinceHittingPlayer");

    void SetTimeSinceHittingPlayer(SpringManAI enemy, float value) =>
        enemy.Reflect().SetInternalField("timeSinceHittingPlayer", value);

    public void OnSecondarySkillHold(SpringManAI enemy) => enemy.SetAnimationGoServerRpc();

    public void ReleaseSecondarySkill(SpringManAI enemy) => enemy.SetAnimationStopServerRpc();

    public bool IsAbleToMove(SpringManAI enemy) => !this.GetStoppingMovement(enemy) || enemy.IsBehaviourState(CoilHeadState.Idle);

    public bool IsAbleToRotate(SpringManAI enemy) => !this.GetStoppingMovement(enemy) || enemy.IsBehaviourState(CoilHeadState.Idle);

    public float InteractRange(SpringManAI _) => 1.5f;

    public void OnOutsideStatusChange(SpringManAI enemy) => enemy.StopSearch(enemy.searchForPlayers, true);

}

