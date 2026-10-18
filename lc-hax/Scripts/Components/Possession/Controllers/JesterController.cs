using System;
using GameNetcodeStuff;
using UnityEngine;

enum JesterState {
    CLOSED,
    CRANKING,
    OPEN
}

sealed class JesterController(JesterAI enemy) : IEnemyController {
    JesterAI Enemy { get; } = enemy;

    Vector3 IEnemyController.PositionOffset => Vector3.zero;

    float IEnemyController.SprintMultiplier => 2.8f;

    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake() is { speed: > 0.0f } agent ? agent.speed : null;

    Vector3 IEnemyController.CameraOffset => this.Enemy.IsBehaviourState(JesterState.OPEN)
        ? new(0.0f, 2.3f, -3.5f)
        : new(0.0f, 2.0f, -3.0f);

    void SetNoPlayerChasetimer(JesterAI enemy, float value) => enemy.noPlayersToChaseTimer = value;

    void IEnemyController.OnPrimaryPressed() {
        if (this.Enemy.inKillAnimation) return;

        this.Enemy.SetBehaviourState(JesterState.CLOSED);
        this.SetNoPlayerChasetimer(this.Enemy, 0.0f);
        this.Enemy.mainCollider.isTrigger = false;
    }

    void IEnemyController.OnSecondaryHeld() {
        if (this.Enemy.inKillAnimation) return;
        if (!this.Enemy.IsBehaviourState(JesterState.CLOSED)) return;
        this.Enemy.SetBehaviourState(JesterState.CRANKING);
    }

    void IEnemyController.ReleaseSecondarySkill() {
        if (!this.Enemy.IsBehaviourState(JesterState.CRANKING)) return;
        this.Enemy.SetBehaviourState(JesterState.OPEN);
        this.Enemy.mainCollider.isTrigger = true;
    }

    void IEnemyController.UpdateManual() => this.SetNoPlayerChasetimer(this.Enemy, 100.0f);

    void IEnemyController.OnUnpossess(PossessionEndReason reason) => this.SetNoPlayerChasetimer(this.Enemy, 5.0f);

    bool IEnemyController.CanMove => !this.Enemy.inKillAnimation;

    bool IEnemyController.CanRotate => !this.Enemy.inKillAnimation;

    string IEnemyController.PrimarySkillName => "Close box";

    string IEnemyController.SecondarySkillName => "(HOLD) Begin cranking";

    float IEnemyController.EntranceRange => 1.0f;

    void IEnemyController.OnOutsideStatusChange() => this.Enemy.StopSearch(this.Enemy.roamMap, true);

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (this.Enemy.inKillAnimation) return;
        if (!this.Enemy.IsBehaviourState(JesterState.OPEN)) return;

        this.Enemy.KillPlayerServerRpc(player.PlayerIndex());
        this.Enemy.inKillAnimation = true;
    }

    void IEnemyController.OnPossess() { }

    void IDisposable.Dispose() { }

    void IEnemyController.Tick(bool inputPaused) { }

    bool IEnemyController.RequiresServerAI() => false;

    void IEnemyController.OnSecondaryPressed() { }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}

