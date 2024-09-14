#region

using Hax;
using UnityEngine;

#endregion

enum CoilHeadState {
    Idle = 0,
    Chase = 1
}

class CoilHeadController : IEnemyController<SpringManAI> {
    Vector3 CamOffset { get; } = new(0, 2.8f, -3.5f);

    public Vector3 GetCameraOffset(SpringManAI _) => this.CamOffset;

    public void UsePrimarySkill(SpringManAI enemy) => enemy.SetBehaviourState(enemy.IsBehaviourState(CoilHeadState.Chase) ? CoilHeadState.Idle : CoilHeadState.Chase);
    
    public void OnSecondarySkillHold(SpringManAI enemy) => enemy.SetAnimationGoServerRpc();

    public void ReleaseSecondarySkill(SpringManAI enemy) => enemy.SetAnimationStopServerRpc();
    
    public void UseSpecialAbility(SpringManAI enemy) {
        if (!this.GetOnCooldown(enemy)) {
            enemy.SetCoilheadOnCooldownServerRpc(true);
            enemy.onCooldownPhase = 999f;
        }
        else {
            enemy.SetCoilheadOnCooldownServerRpc(false);
        }
    }
    
    public void Update(SpringManAI enemy, bool isAIControlled) {
        if (isAIControlled) return;

        if (enemy.IsBehaviourState(CoilHeadState.Chase)) {
            enemy.Reflect().SetInternalField("loseAggroTimer", 0);
            enemy.timeSpentMoving = 0f;
        }
    }

    public bool IsAbleToMove(SpringManAI enemy) => !this.GetStoppingMovement(enemy) ||
                                                   (enemy.IsBehaviourState(CoilHeadState.Idle) &&
                                                    enemy.agent.speed >= 0) && !this.GetOnCooldown(enemy);

    public bool IsAbleToRotate(SpringManAI enemy) => !this.GetStoppingMovement(enemy) ||
                                                     (enemy.IsBehaviourState(CoilHeadState.Idle) &&
                                                      enemy.agent.speed >= 0) && !this.GetOnCooldown(enemy);

    public void OnOutsideStatusChange(SpringManAI enemy) => enemy.StopSearch(enemy.searchForPlayers, true);

    bool GetStoppingMovement(SpringManAI enemy) => enemy.Reflect().GetInternalField<bool>("stoppingMovement");

    bool GetOnCooldown(SpringManAI enemy) => enemy.Reflect().GetInternalField<bool>("setOnCooldown");

    public string GetSpecialAbilityName(SpringManAI enemy) => this.GetOnCooldown(enemy) ? "Awake" : "Sleep";

    public string GetPrimarySkillName(SpringManAI enemy) =>
        enemy.IsBehaviourState(CoilHeadState.Chase) ? "Idle" : "Chase";

    public string GetSecondarySkillName(SpringManAI _) => "Toggle Animation";

}
