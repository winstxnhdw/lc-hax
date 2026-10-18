using System;
using GameNetcodeStuff;
using UnityEngine;

sealed class ButlerBeesController(ButlerBeesEnemyAI enemy) : IEnemyController {
    const float StingCooldown = 0.5f;

    ButlerBeesEnemyAI Enemy { get; } = enemy;
    float? OriginalBuzzPitch { get; } = enemy.buzzing.Unfake()?.pitch;

    Vector3 IEnemyController.CameraOffset => new(0.0f, 2.0f, -3.0f);
    Vector3 IEnemyController.PositionOffset => Vector3.zero;
    bool IEnemyController.CanMove => true;
    bool IEnemyController.CanRotate => true;
    float IEnemyController.EntranceRange => 2.5f;
    float IEnemyController.SprintMultiplier => 1.8f;
    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake() is { speed: > 0.0f } agent ? agent.speed : null;
    string? IEnemyController.PrimarySkillName => null;
    string? IEnemyController.SecondarySkillName => null;

    void IEnemyController.UpdateManual() {
        this.StopSearch();
        this.Enemy.movingTowardsTargetPlayer = false;
        this.Enemy.chasePlayerTimer = 0.0f;
    }

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (Time.realtimeSinceStartup - this.Enemy.timeAtLastHurtingPlayer < ButlerBeesController.StingCooldown) return;

        this.Enemy.timeAtLastHurtingPlayer = Time.realtimeSinceStartup;
        player.DamagePlayerRpc(10);
    }

    void StopSearch() {
        if (this.Enemy.currentSearch is not { inProgress: true } search) return;
        this.Enemy.StopSearch(search, true);
    }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) {
        if (this.Enemy.Unfake() is not { IsSpawned: true, isEnemyDead: false } enemy) return;

        if (this.OriginalBuzzPitch is float pitch && enemy.buzzing.Unfake() is AudioSource buzzing) {
            buzzing.pitch = pitch;
        }

        if (!enemy.IsOwner) {
            return;
        }

        enemy.movingTowardsTargetPlayer = false;
        enemy.chasePlayerTimer = 0.0f;
    }

    void IEnemyController.OnOutsideStatusChange() => this.StopSearch();

    void IDisposable.Dispose() { }

    void IEnemyController.OnPossess() { }

    void IEnemyController.Tick(bool inputPaused) { }

    bool IEnemyController.RequiresServerAI() => false;

    void IEnemyController.OnPrimaryPressed() { }

    void IEnemyController.OnSecondaryPressed() { }

    void IEnemyController.OnSecondaryHeld() { }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.ReleaseSecondarySkill() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) {
        if (this.Enemy.buzzing.Unfake() is not AudioSource buzzing) return;

        buzzing.pitch = Mathf.Lerp(buzzing.pitch, isSprinting ? 1.3f : 1.0f, Time.deltaTime * 5.0f);
    }
}
