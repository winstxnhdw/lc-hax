using System;
using GameNetcodeStuff;
using UnityEngine;

enum BeesState {
    IDLE,
    DEFENSIVE,
    ATTACK
}

sealed class CircuitBeesController(RedLocustBees enemy) : IEnemyController {
    RedLocustBees Enemy { get; } = enemy;

    Vector3 IEnemyController.PositionOffset => Vector3.zero;

    bool IEnemyController.CanMove => true;

    bool IEnemyController.CanRotate => true;

    float IEnemyController.EntranceRange => 2.5f;

    float IEnemyController.SprintMultiplier => 2.8f;

    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake()?.speed;

    string? IEnemyController.PrimarySkillName => null;

    string? IEnemyController.SecondarySkillName => null;

    bool IEnemyController.AllowAIToChangeOwner => false;

    Vector3 IEnemyController.CameraOffset => new(0.0f, 2.0f, -3.0f);

    void IEnemyController.OnOutsideStatusChange() => this.Enemy.StopSearch(this.Enemy.searchForHive, true);

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (this.Enemy.timeSinceHittingPlayer < 0.4f) return;

        this.Enemy.timeSinceHittingPlayer = 0.0f;

        if (player.health <= 10 || player.criticallyInjured) {
            this.Enemy.BeeKillPlayerServerRpc(player.PlayerIndex());
        }

        else {
            player.DamagePlayerRpc(10);
        }

        this.Enemy.beesZappingMode = 3;
        this.Enemy.EnterAttackZapModeServerRpc(-1);
    }

    void IEnemyController.OnPrimaryPressed() {
        this.Enemy.SetBehaviourState(BeesState.ATTACK);
        this.Enemy.EnterAttackZapModeServerRpc(-1);
    }

    void IEnemyController.OnSecondaryPressed() => this.Enemy.SetBehaviourState(BeesState.IDLE);

    void IEnemyController.OnPossess() { }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) { }

    void IDisposable.Dispose() { }

    void IEnemyController.Tick(bool inputPaused) { }

    void IEnemyController.UpdateManual() { }

    void IEnemyController.OnAIControlChanged(bool isAIControlled) { }

    bool IEnemyController.RequiresServerAI(bool isAIControlled) => false;

    void IEnemyController.OnSecondaryHeld() { }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.ReleaseSecondarySkill() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}

