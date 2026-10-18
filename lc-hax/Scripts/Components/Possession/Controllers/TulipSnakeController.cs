using System;
using GameNetcodeStuff;
using UnityEngine;

sealed class TulipSnakeController(FlowerSnakeEnemy enemy) : IEnemyController {
    FlowerSnakeEnemy Enemy { get; } = enemy;

    Vector3 IEnemyController.CameraOffset => new(0.0f, 2.0f, -3.0f);
    Vector3 IEnemyController.PositionOffset => Vector3.zero;
    float IEnemyController.EntranceRange => 1.5f;
    float IEnemyController.SprintMultiplier => 1.5f;
    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake() is { speed: > 0.0f } agent ? agent.speed : null;

    bool CanWalk => this.Enemy.clingingToPlayer is null && !this.Enemy.leaping &&
        !this.Enemy.fallingFromLeap && !this.Enemy.daytimeEnemyLeaving && this.Enemy.stunNormalizedTimer <= 0.0f;

    bool IEnemyController.CanMove => this.CanWalk;
    bool IEnemyController.CanRotate => this.CanWalk;
    string? IEnemyController.PrimarySkillName => this.Enemy.clingingToPlayer is null ? null : "Release player";

    string? IEnemyController.SecondarySkillName => null;

    void IEnemyController.OnPossess() => this.Enemy.StopSearch(this.Enemy.snakeRoam, true);

    void IEnemyController.OnUnpossess(PossessionEndReason reason) {
        if (!this.Enemy.IsOwner) return;
        if (this.Enemy.isEnemyDead) return;

        this.ReleasePlayer();
    }

    void IEnemyController.OnPrimaryPressed() => this.ReleasePlayer();

    void IEnemyController.OnSecondaryPressed() { }

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (this.Enemy.clingingToPlayer is not null) return;
        if (this.Enemy.daytimeEnemyLeaving) return;
        if (this.Enemy.stunNormalizedTimer > 0.0f) return;
        if (this.Enemy.waitingForHitPlayerRPC) return;
        if (Time.realtimeSinceStartup - this.Enemy.collideWithPlayerInterval < 1.0f) return;
        if (Time.realtimeSinceStartup - this.Enemy.timeOfLastCling < 4.0f) return;
        if (player.inAnimationWithEnemy) return;
        if (player.isInElevator && Helper.StartOfRound is { shipIsLeaving: true }) return;

        this.Enemy.collideWithPlayerInterval = Time.realtimeSinceStartup;
        this.Enemy.FSHitPlayerServerRpc(player.PlayerIndex());
    }

    void ReleasePlayer() {
        if (this.Enemy.clingingToPlayer is null) return;
        if (Helper.LocalPlayer is not { } sender) return;

        this.Enemy.StopClingingOnLocalClient(true);
        this.Enemy.StopClingingServerRpc(sender.PlayerIndex());
    }

    void IEnemyController.OnOutsideStatusChange() => this.Enemy.StopSearch(this.Enemy.snakeRoam, true);

    bool IEnemyController.RequiresServerAI() => false;
    void IEnemyController.Tick(bool inputPaused) { }
    void IEnemyController.UpdateManual() { }
    void IEnemyController.OnSecondaryHeld() { }
    void IEnemyController.CancelSecondarySkill() { }
    void IEnemyController.ReleaseSecondarySkill() { }
    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
    void IDisposable.Dispose() { }
}
