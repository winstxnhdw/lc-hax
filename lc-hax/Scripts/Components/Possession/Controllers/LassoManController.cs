using System;
using GameNetcodeStuff;
using UnityEngine;

sealed class LassoManController(LassoManAI enemy) : IEnemyController {
    LassoManAI Enemy { get; } = enemy;

    Vector3 IEnemyController.PositionOffset => Vector3.zero;

    bool IEnemyController.CanMove => true;

    bool IEnemyController.CanRotate => true;

    float IEnemyController.EntranceRange => 2.5f;

    float IEnemyController.SprintMultiplier => 2.8f;

    string? IEnemyController.PrimarySkillName => null;

    string? IEnemyController.SecondarySkillName => null;

    Vector3 IEnemyController.CameraOffset => new(0.0f, 2.8f, -3.5f);

    void IEnemyController.OnPrimaryPressed() => this.Enemy.MakeScreechNoiseServerRpc();

    float? IEnemyController.MovementSpeed => null;

    void IEnemyController.OnOutsideStatusChange() => this.Enemy.StopSearch(this.Enemy.searchForPlayers, true);

    void IEnemyController.OnPossess() { }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) { }

    void IDisposable.Dispose() { }

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) { }

    void IEnemyController.Tick(bool inputPaused) { }

    void IEnemyController.UpdateManual() { }

    bool IEnemyController.RequiresServerAI() => false;

    void IEnemyController.OnSecondaryPressed() { }

    void IEnemyController.OnSecondaryHeld() { }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.ReleaseSecondarySkill() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}

