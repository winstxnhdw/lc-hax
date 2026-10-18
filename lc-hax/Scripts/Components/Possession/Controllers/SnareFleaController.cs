using System;
using GameNetcodeStuff;
using UnityEngine;

enum SnareFleaState {
    SEARCHING,
    HIDING,
    CHASING,
    CLINGING
}

sealed class SnareFleaController(CentipedeAI enemy) : IEnemyController {
    CentipedeAI Enemy { get; } = enemy;

    Vector3 IEnemyController.PositionOffset => Vector3.zero;

    float IEnemyController.SprintMultiplier => 2.8f;

    bool IEnemyController.AllowAIToChangeOwner => false;

    Vector3 IEnemyController.CameraOffset => this.Enemy.clingingToPlayer is not null
        ? new(0.0f, 0.0f, -2.0f)
        : this.Enemy.IsBehaviourState(SnareFleaState.HIDING) ? new(0.0f, 2.5f, 0.0f) : new(0.0f, 2.0f, -4.0f);

    bool IsClingingToSomething(CentipedeAI enemy) {
        return enemy is { clingingToPlayer: not null }
            or { inSpecialAnimation: true }
            or { clingingToDeadBody: true }
            or { clingingToCeiling: true }
            or { startedCeilingAnimationCoroutine: true }
            or { inDroppingOffPlayerAnim: true };
    }

    void IEnemyController.OnPrimaryPressed() {
        if (!this.Enemy.IsBehaviourState(SnareFleaState.HIDING)) return;
        this.Enemy.SetBehaviourState(SnareFleaState.CHASING);
    }

    void IEnemyController.OnSecondaryPressed() {
        if (this.IsClingingToSomething(this.Enemy)) return;
        this.Enemy.RaycastToCeiling();
    }

    bool IEnemyController.CanMove => !this.IsClingingToSomething(this.Enemy);

    bool IEnemyController.CanRotate => !this.IsClingingToSomething(this.Enemy);

    string IEnemyController.PrimarySkillName => "Drop";

    string IEnemyController.SecondarySkillName => "Attach to ceiling";

    float IEnemyController.EntranceRange => this.IsClingingToSomething(this.Enemy) ? 0.0f : 1.5f;

    float? IEnemyController.MovementSpeed => null;

    void IEnemyController.OnPossess() { }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) { }

    void IDisposable.Dispose() { }

    void IEnemyController.OnOutsideStatusChange() { }

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) { }

    void IEnemyController.Tick(bool inputPaused) { }

    void IEnemyController.UpdateManual() { }

    void IEnemyController.OnAIControlChanged(bool isAIControlled) { }

    bool IEnemyController.RequiresServerAI(bool isAIControlled) => false;

    void IEnemyController.OnSecondaryHeld() { }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.ReleaseSecondarySkill() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}
