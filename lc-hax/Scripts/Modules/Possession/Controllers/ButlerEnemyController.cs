using GameNetcodeStuff;
using Hax;
using UnityEngine;

public enum ButlerBehaviorState {
    Idle = 0,
    Alert = 1,
    Murder = 2
}

internal class ButlerEnemyController : IEnemyController<ButlerEnemyAI> {

    Vector3 camOffset = new(0, 2.8f, -3f);

    public Vector3 GetCameraOffset(ButlerEnemyAI enemy) => this.camOffset;

    public void OnMovement(ButlerEnemyAI enemy, bool isMoving, bool isSprinting) => enemy.SetButlerRunningServerRpc(isSprinting);



    public void OnPrimarySkillHold(ButlerEnemyAI enemy) {
        enemy.SetBehaviourState(ButlerBehaviorState.Murder);
        enemy.StabPlayerServerRpc(enemy.FindClosestPlayer(4f).PlayerIndex(), Random.value < 0.8f);
    }


    public void ReleasePrimarySkill(ButlerEnemyAI enemy) {
        _ = enemy.Reflect().InvokeInternalMethod("ForgetSeenPlayers");
        enemy.SetBehaviourState(ButlerBehaviorState.Idle);
    }
    public void UseSecondarySkill(ButlerEnemyAI enemy) {
        enemy.SetBehaviourState(ButlerBehaviorState.Idle);
        _ = enemy.Reflect().SetInternalField("sweepFloorTimer", 10f);
        enemy.SyncSearchingMadlyServerRpc(false);
        enemy.SetSweepingAnimServerRpc(true);

    }
    public bool IsAbleToMove(ButlerEnemyAI enemy) => !enemy.Reflect().GetInternalField<bool>("currentSpecialAnimation");

    public bool IsAbleToRotate(ButlerEnemyAI enemy) => !enemy.Reflect().GetInternalField<bool>("currentSpecialAnimation");

    public bool SyncAnimationSpeedEnabled(ButlerEnemyAI _) => false;

    public void OnOutsideStatusChange(ButlerEnemyAI enemy) {
        enemy.StopSearch(enemy.roamAndSweepFloor, true);
        enemy.StopSearch(enemy.hoverAroundTargetPlayer, true);
    }
}
