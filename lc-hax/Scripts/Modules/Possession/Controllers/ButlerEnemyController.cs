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

    public void UsePrimarySkill(ButlerEnemyAI enemy) {
        _ = enemy.Reflect().InvokeInternalMethod("ForgetSeenPlayers");
        enemy.StabPlayerServerRpc(enemy.GetClosestPlayer().PlayerIndex(), true);

    }
    public void UseSecondarySkill(ButlerEnemyAI enemy) => enemy.SetSweepingAnimServerRpc(enemy.Reflect().GetInternalField<bool>("isSweeping"));

    public bool IsAbleToMove(ButlerEnemyAI enemy) => !enemy.Reflect().GetInternalField<bool>("currentSpecialAnimation");

    public bool IsAbleToRotate(ButlerEnemyAI enemy) => !enemy.Reflect().GetInternalField<bool>("currentSpecialAnimation");

    public bool SyncAnimationSpeedEnabled(ButlerEnemyAI _) => false;

    public void OnOutsideStatusChange(ButlerEnemyAI enemy) {
        enemy.StopSearch(enemy.roamAndSweepFloor, true);
        enemy.StopSearch(enemy.hoverAroundTargetPlayer, true);
    }
}
