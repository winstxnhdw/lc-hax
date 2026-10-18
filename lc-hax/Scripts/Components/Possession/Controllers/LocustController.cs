using System;
using GameNetcodeStuff;
using UnityEngine;

enum LocustState {
    ROAMING,
    HIDING
}

sealed class LocustController(DocileLocustBeesAI enemy) : IEnemyController {
    DocileLocustBeesAI Enemy { get; } = enemy;

    bool IEnemyController.CanMove => true;

    bool IEnemyController.CanRotate => true;

    float IEnemyController.EntranceRange => 2.5f;

    float IEnemyController.SprintMultiplier => 2.8f;

    string? IEnemyController.PrimarySkillName => null;

    bool IEnemyController.AllowAIToChangeOwner => false;

    bool IsHiding { get; set; }

    Vector3 IEnemyController.CameraOffset => new(0.0f, 2.0f, -4.5f);

    Vector3 IEnemyController.PositionOffset => new(0.0f, 1.5f, 0.0f);

    void IEnemyController.OnSecondaryPressed() => this.IsHiding = true;

    void IEnemyController.CancelSecondarySkill() => this.IsHiding = false;

    string IEnemyController.SecondarySkillName => "(HOLD) Hide";

    float? IEnemyController.MovementSpeed => null;

    void IEnemyController.UpdateManual() => this.Enemy.SetBehaviourState(
        this.IsHiding ? LocustState.HIDING : LocustState.ROAMING
    );

    void IEnemyController.OnOutsideStatusChange() => this.Enemy.StopSearch(this.Enemy.bugsRoam, true);

    void IEnemyController.OnPossess() { }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) { }

    void IDisposable.Dispose() { }

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) { }

    void IEnemyController.Tick(bool inputPaused) { }

    void IEnemyController.OnAIControlChanged(bool isAIControlled) { }

    bool IEnemyController.RequiresServerAI(bool isAIControlled) => false;

    void IEnemyController.OnPrimaryPressed() { }

    void IEnemyController.OnSecondaryHeld() { }

    void IEnemyController.ReleaseSecondarySkill() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}
