using System;
using GameNetcodeStuff;
using UnityEngine;

sealed class TestEnemyController(TestEnemy enemy) : IEnemyController {
    TestEnemy Enemy { get; } = enemy;

    bool IEnemyController.CanMove => true;

    bool IEnemyController.CanRotate => true;

    float IEnemyController.SprintMultiplier => 2.8f;

    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake()?.speed;

    string? IEnemyController.PrimarySkillName => null;

    string? IEnemyController.SecondarySkillName => null;

    Vector3 IEnemyController.CameraOffset => new(0.0f, 2.0f, -4.5f);

    Vector3 IEnemyController.PositionOffset => new(0.0f, 1.5f, 0.0f);

    float IEnemyController.EntranceRange => 4.5f;

    void IEnemyController.OnPossess() { }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) { }

    void IDisposable.Dispose() { }

    void IEnemyController.OnOutsideStatusChange() { }

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) { }

    void IEnemyController.Tick(bool inputPaused) { }

    void IEnemyController.UpdateManual() { }

    bool IEnemyController.RequiresServerAI() => false;

    void IEnemyController.OnPrimaryPressed() { }

    void IEnemyController.OnSecondaryPressed() { }

    void IEnemyController.OnSecondaryHeld() { }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.ReleaseSecondarySkill() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}

