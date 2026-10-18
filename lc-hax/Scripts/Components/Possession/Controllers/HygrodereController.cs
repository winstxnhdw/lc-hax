using System;
using GameNetcodeStuff;
using UnityEngine;

sealed class HygrodereController(BlobAI enemy) : IEnemyController {
    BlobAI Enemy { get; } = enemy;

    Vector3 IEnemyController.PositionOffset => Vector3.zero;

    bool IEnemyController.CanMove => true;

    bool IEnemyController.CanRotate => true;

    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake() is { speed: > 0.0f } agent ? agent.speed : null;

    string IEnemyController.PrimarySkillName => "Anger";

    string? IEnemyController.SecondarySkillName => null;

    Vector3 IEnemyController.CameraOffset => new(0.0f, 2.0f, -3.0f);

    void IEnemyController.OnPrimaryPressed() => this.Enemy.HitEnemyOnLocalClient(force: 0);

    float IEnemyController.EntranceRange => 3.5f;

    float IEnemyController.SprintMultiplier => 9.8f;

    void IEnemyController.OnUnpossess(PossessionEndReason reason) { }

    void IEnemyController.OnOutsideStatusChange() => this.Enemy.StopSearch(this.Enemy.searchForPlayers, true);

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (this.Enemy.timeSinceHittingLocalPlayer < 0.25f) return;
        if (this.Enemy.tamedTimer > 0.0f && this.Enemy.angeredTimer <= 0.0f) return;

        this.Enemy.timeSinceHittingLocalPlayer = 0.0f;
        player.DamagePlayerRpc(35);

        if (player.health <= 35 && player.criticallyInjured) {
            this.Enemy.SlimeKillPlayerEffectServerRpc(player.PlayerIndex());
        }
    }

    void IEnemyController.OnPossess() { }

    void IDisposable.Dispose() { }

    void IEnemyController.Tick(bool inputPaused) { }

    void IEnemyController.UpdateManual() { }

    bool IEnemyController.RequiresServerAI() => false;

    void IEnemyController.OnSecondaryPressed() { }

    void IEnemyController.OnSecondaryHeld() { }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.ReleaseSecondarySkill() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}

