using System;
using GameNetcodeStuff;
using UnityEngine;

sealed class MaskedController(MaskedPlayerEnemy enemy) : IEnemyController {
    MaskedPlayerEnemy Enemy { get; } = enemy;

    Vector3 IEnemyController.PositionOffset => Vector3.zero;

    float IEnemyController.SprintMultiplier => 2.8f;

    string? IEnemyController.PrimarySkillName => null;

    string? IEnemyController.SecondarySkillName => null;

    bool IEnemyController.AllowAIToChangeOwner => false;

    Vector3 IEnemyController.CameraOffset => new(0.0f, 2.8f, -3.0f);

    void IEnemyController.OnPrimaryPressed() => this.Enemy.SetHandsOutServerRpc(
        !this.Enemy.creatureAnimator.GetBool("HandsOut")
    );

    void IEnemyController.OnSecondaryPressed() => this.Enemy.SetCrouchingServerRpc(
        !this.Enemy.creatureAnimator.GetBool("Crouching")
    );

    float IEnemyController.EntranceRange => 1.0f;

    float? IEnemyController.MovementSpeed => null;

    bool IEnemyController.CanMove => !this.Enemy.inKillAnimation;

    bool IEnemyController.CanRotate => !this.Enemy.inKillAnimation;

    void IEnemyController.OnOutsideStatusChange() {
        this.Enemy.StopSearch(this.Enemy.searchForPlayers, true);
        this.Enemy.TeleportMaskedEnemyServerRpc(this.Enemy.transform.position, this.Enemy.isOutside);
    }

    void IEnemyController.OnPossess() { }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) { }

    void IDisposable.Dispose() { }

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
