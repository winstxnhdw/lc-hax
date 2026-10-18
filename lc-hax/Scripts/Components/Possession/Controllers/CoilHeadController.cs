using System;
using GameNetcodeStuff;
using UnityEngine;

enum CoilHeadState {
    IDLE,
    CHASE
}

sealed class CoilHeadController(SpringManAI enemy) : IEnemyController {
    SpringManAI Enemy { get; } = enemy;

    Vector3 IEnemyController.PositionOffset => Vector3.zero;

    float IEnemyController.SprintMultiplier => 2.8f;

    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake() is { speed: > 0.0f } agent ? agent.speed : null;

    bool IsUsingSecondarySkill { get; set; }

    Vector3 IEnemyController.CameraOffset => new(0.0f, 2.8f, -3.5f);

    void IEnemyController.UpdateManual() {
        bool wasOnCooldown = this.Enemy.setOnCooldown;

        if (wasOnCooldown) {
            this.Enemy.onCooldownPhase = 0.0f;
            this.Enemy.setOnCooldown = false;
            this.Enemy.SetCoilheadOnCooldownServerRpc(false);
        }

        if (wasOnCooldown && this.Enemy.previousBehaviourStateIndex == (int)CoilHeadState.CHASE) {
            this.Enemy.SetBehaviourState(CoilHeadState.CHASE);
        }

        if (wasOnCooldown && this.IsUsingSecondarySkill) {
            this.Enemy.SetAnimationGoServerRpc();
        }

        if (this.IsUsingSecondarySkill) {
            this.Enemy.stopAndGoMinimumInterval = 0.25f;
            this.Enemy.stoppingMovement = false;
        }
    }

    void IEnemyController.OnPrimaryPressed() => this.Enemy.SetBehaviourState(
        this.Enemy.IsBehaviourState(CoilHeadState.CHASE) ? CoilHeadState.IDLE : CoilHeadState.CHASE
    );

    string IEnemyController.PrimarySkillName => this.Enemy.IsBehaviourState(CoilHeadState.CHASE) ? "Roam" : "Chase";

    void IEnemyController.OnSecondaryHeld() {
        if (this.IsUsingSecondarySkill) return;

        this.IsUsingSecondarySkill = true;
        this.Enemy.SetAnimationGoServerRpc();
    }

    void IEnemyController.CancelSecondarySkill() => this.IsUsingSecondarySkill = false;

    void IEnemyController.ReleaseSecondarySkill() => this.Enemy.SetAnimationStopServerRpc();

    void IEnemyController.OnUnpossess(PossessionEndReason reason) => this.Enemy.stopAndGoMinimumInterval = 0.0f;

    string IEnemyController.SecondarySkillName => "(HOLD) Move while observed";

    bool IEnemyController.CanMove => !this.Enemy.stoppingMovement;

    bool IEnemyController.CanRotate => !this.Enemy.stoppingMovement;

    float IEnemyController.EntranceRange => 1.5f;

    void IEnemyController.OnOutsideStatusChange() => this.Enemy.StopSearch(this.Enemy.searchForPlayers, true);

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (this.Enemy.stoppingMovement) return;
        if (this.Enemy.hitPlayerTimer >= 0.0f) return;
        if (this.Enemy.setOnCooldown) return;
        if (!this.Enemy.IsBehaviourState(CoilHeadState.CHASE)) return;
        if (Time.realtimeSinceStartup - this.Enemy.timeAtLastCooldown < 0.45f) return;

        this.Enemy.hitPlayerTimer = 0.2f;
        this.Enemy.timeSinceHittingPlayer = Time.realtimeSinceStartup;
        player.DamagePlayerRpc(90);
    }

    void IEnemyController.OnPossess() { }

    void IDisposable.Dispose() { }

    void IEnemyController.Tick(bool inputPaused) { }

    bool IEnemyController.RequiresServerAI() => false;

    void IEnemyController.OnSecondaryPressed() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}

