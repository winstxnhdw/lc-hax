using GameNetcodeStuff;
using Hax;
using UnityEngine;

enum CoilHeadState {
    Idle = 0,
    Chase = 1
}

internal class CoilHeadController : IEnemyController<SpringManAI> {
    bool GetStoppingMovement(SpringManAI enemy) => enemy.Reflect().GetInternalField<bool>("stoppingMovement");

    float GetTimeSinceHittingPlayer(SpringManAI enemy) =>
        enemy.Reflect().GetInternalField<float>("timeSinceHittingPlayer");

    void SetTimeSinceHittingPlayer(SpringManAI enemy, float value) =>
        enemy.Reflect().SetInternalField("timeSinceHittingPlayer", value);

    public void OnSecondarySkillHold(SpringManAI enemy) => enemy.SetAnimationGoServerRpc();

    public void ReleaseSecondarySkill(SpringManAI enemy) => enemy.SetAnimationStopServerRpc();

    public bool IsAbleToMove(SpringManAI enemy) => !this.GetStoppingMovement(enemy);

    public bool IsAbleToRotate(SpringManAI enemy) => !this.GetStoppingMovement(enemy);

    public float InteractRange(SpringManAI _) => 1.5f;

    public void OnOutsideStatusChange(SpringManAI enemy) => enemy.StopSearch(enemy.searchForPlayers, true);

    public void OnCollideWithPlayer(SpringManAI enemy, PlayerControllerB player) {
        if (enemy.isOutside) {

            if(this.GetStoppingMovement(enemy)) return;
            if(!enemy.IsBehaviourState(CoilHeadState.Chase)) return;
            if(this.GetTimeSinceHittingPlayer(enemy) >= 0f) return;
            {
                this.SetTimeSinceHittingPlayer(enemy, 0.2f);
                player.DamagePlayer(90, true, true, CauseOfDeath.Mauling, 2, false, default);
                player.JumpToFearLevel(1f, true);
            }
        }
    }
}

