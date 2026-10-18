using System;
using GameNetcodeStuff;
using UnityEngine;

sealed class EarthLeviathanController(SandWormAI enemy) : IEnemyController {
    SandWormAI Enemy { get; } = enemy;

    Vector3 IEnemyController.PositionOffset => Vector3.zero;

    bool IEnemyController.CanMove => true;

    bool IEnemyController.CanRotate => true;

    float IEnemyController.SprintMultiplier => 2.8f;

    string? IEnemyController.PrimarySkillName => null;

    string IEnemyController.SecondarySkillName => "Emerge";

    float IEnemyController.EntranceRange => this.Enemy is { inEmergingState: true } or { emerged: true } or { inSpecialAnimation: true } ? 0.0f : 2.5f;

    float? IEnemyController.MovementSpeed => null;

    Vector3 IEnemyController.CameraOffset => new(0.0f, 8.0f, -13.0f);

    void IEnemyController.OnSecondaryPressed() {
        if (this.Enemy is { inEmergingState: true } or { emerged: true }) return;

        this.Enemy.SyncPositionToClients();
        this.Enemy.StartEmergeAnimation();
    }

    void IEnemyController.OnOutsideStatusChange() => this.Enemy.StopSearch(this.Enemy.roamMap, true);

    void IEnemyController.OnPossess() { }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) { }

    void IDisposable.Dispose() { }

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) { }

    void IEnemyController.Tick(bool inputPaused) { }

    void IEnemyController.UpdateManual() { }

    bool IEnemyController.RequiresServerAI() => false;

    void IEnemyController.OnPrimaryPressed() { }

    void IEnemyController.OnSecondaryHeld() { }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.ReleaseSecondarySkill() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}
