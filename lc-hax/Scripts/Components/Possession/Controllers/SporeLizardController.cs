using System;
using GameNetcodeStuff;
using UnityEngine;

enum SporeLizardState {
    IDLE,
    ALERTED,
    HOSTILE
}

sealed class SporeLizardController(PufferAI enemy) : IEnemyController {
    PufferAI Enemy { get; } = enemy;

    Vector3 IEnemyController.CameraOffset => new(0.0f, 3.0f, -3.0f);

    Vector3 IEnemyController.PositionOffset => Vector3.zero;

    float IEnemyController.EntranceRange => 2.5f;

    float IEnemyController.SprintMultiplier => 2.8f;

    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake() is { speed: > 0.0f } agent ? agent.speed : null;

    void IEnemyController.OnPrimaryPressed() {
        if (this.Enemy.inPuffingAnimation) return;

        this.Enemy.SetBehaviourState(SporeLizardState.HOSTILE);
        this.Enemy.StompServerRpc();
    }

    void IEnemyController.OnSecondaryPressed() {
        if (this.Enemy.inPuffingAnimation) return;

        this.Enemy.SetBehaviourState(SporeLizardState.HOSTILE);
        this.Enemy.ShakeTailServerRpc();
    }

    string IEnemyController.PrimarySkillName => "Stomp";

    string IEnemyController.SecondarySkillName => "Smoke";

    bool IEnemyController.CanMove => true;

    bool IEnemyController.CanRotate => true;

    void IEnemyController.OnOutsideStatusChange() => this.Enemy.StopSearch(this.Enemy.roamMap, true);

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (this.Enemy.timeSinceHittingPlayer <= 1.0f) return;

        this.Enemy.timeSinceHittingPlayer = 0.0f;
        player.DamagePlayerRpc(20);
        this.Enemy.BitePlayerServerRpc(player.PlayerIndex());
    }

    void IEnemyController.OnPossess() { }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) { }

    void IDisposable.Dispose() { }

    void IEnemyController.Tick(bool inputPaused) { }

    void IEnemyController.UpdateManual() { }

    bool IEnemyController.RequiresServerAI() => false;

    void IEnemyController.OnSecondaryHeld() { }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.ReleaseSecondarySkill() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}
