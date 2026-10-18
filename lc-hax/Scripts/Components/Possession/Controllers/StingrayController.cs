using System;
using GameNetcodeStuff;
using UnityEngine;

sealed class StingrayController(StingrayAI enemy) : IEnemyController {
    const int RoamingState = 0;
    const int CloakedState = 1;

    StingrayAI Enemy { get; } = enemy;

    Vector3 IEnemyController.CameraOffset => new(0.0f, 2.0f, -3.5f);
    Vector3 IEnemyController.PositionOffset => Vector3.zero;
    bool IEnemyController.CanMove => this.Enemy.currentBehaviourStateIndex != StingrayController.CloakedState && this.Enemy.stunNormalizedTimer < 0.0f;
    bool IEnemyController.CanRotate => this.Enemy.currentBehaviourStateIndex != StingrayController.CloakedState && this.Enemy.stunNormalizedTimer < 0.0f;
    float IEnemyController.EntranceRange => 1.5f;
    float IEnemyController.SprintMultiplier => 2.8f;
    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake() is { speed: > 0.0f } agent ? agent.speed : null;
    string IEnemyController.PrimarySkillName => "Ambush";
    string IEnemyController.SecondarySkillName => "Cloak";

    void IEnemyController.OnPrimaryPressed() {
        if (this.Enemy.currentBehaviourStateIndex != StingrayController.CloakedState) return;
        if (this.Enemy.previousBehaviourState != StingrayController.CloakedState) return;
        if (this.Enemy.stunNormalizedTimer >= 0.0f) return;

        this.Enemy.targetPlayer = null;
        this.Enemy.SwitchToBehaviourState(StingrayController.RoamingState);
    }

    void IEnemyController.OnSecondaryPressed() {
        if (this.Enemy.currentBehaviourStateIndex == StingrayController.CloakedState) return;
        if (this.Enemy.stunNormalizedTimer >= 0.0f) return;
        if (this.Enemy.exitCloakLunge > 0.0f) return;
        if (!this.Enemy.ventAnimationFinished) return;

        this.Enemy.SwitchToBehaviourState(StingrayController.CloakedState);
    }

    void IEnemyController.UpdateManual() {
        this.Enemy.movingTowardsTargetPlayer = false;
        this.Enemy.targetPlayer = null;

        if (this.Enemy.hidingSpot is { } hidingSpot) {
            hidingSpot.gotHidingSpot = false;
        }
    }

    void RestoreAI() {
        if (this.Enemy.Unfake() is not { IsSpawned: true, IsOwner: true, isEnemyDead: false } enemy) return;

        enemy.movingTowardsTargetPlayer = false;
        enemy.targetPlayer = null;
        enemy.targetNode = null;
        enemy.stuckTimer = 0.0f;
        enemy.seePlayerAndHideTimer = 0.0f;
        enemy.updateDestinationInterval = 0.0f;

        if (enemy.hidingSpot is { } hidingSpot) {
            hidingSpot.gotHidingSpot = false;
        }
    }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) => this.RestoreAI();

    void IEnemyController.OnOutsideStatusChange() => this.RestoreAI();

    bool IEnemyController.RequiresServerAI() => false;

    void IEnemyController.OnPossess() { }

    void IDisposable.Dispose() { }

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) { }

    void IEnemyController.Tick(bool inputPaused) { }

    void IEnemyController.OnSecondaryHeld() { }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.ReleaseSecondarySkill() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}
