using System;
using GameNetcodeStuff;
using UnityEngine;

sealed class CrawlerController(CrawlerAI enemy) : IEnemyController {
    CrawlerAI Enemy { get; } = enemy;

    Vector3 IEnemyController.CameraOffset => new(0.0f, 3.0f, -3.0f);

    Vector3 IEnemyController.PositionOffset => Vector3.zero;

    bool IEnemyController.CanMove => true;

    bool IEnemyController.CanRotate => true;

    float IEnemyController.SprintMultiplier => 2.8f;

    string? IEnemyController.PrimarySkillName => null;

    string? IEnemyController.SecondarySkillName => null;

    float IEnemyController.EntranceRange => 1.5f;

    float? IEnemyController.MovementSpeed => null;

    void IEnemyController.OnSecondaryPressed() => this.Enemy.MakeScreechNoiseServerRpc();

    void IEnemyController.OnPrimaryPressed() => this.Enemy.CollideWithWallServerRpc();

    void IEnemyController.OnOutsideStatusChange() => this.Enemy.StopSearch(this.Enemy.searchForPlayers, true);

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (this.Enemy.timeSinceHittingPlayer < 0.65f) return;

        this.Enemy.timeSinceHittingPlayer = 0.0f;
        this.Enemy.agent.speed = 0.0f;
        player.DamagePlayerRpc(40);
        this.Enemy.HitPlayerServerRpc(player.PlayerIndex());
    }

    void IEnemyController.OnPossess() { }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) { }

    void IDisposable.Dispose() { }

    void IEnemyController.Tick(bool inputPaused) { }

    void IEnemyController.UpdateManual() { }

    bool IEnemyController.RequiresServerAI() => false;

    void IEnemyController.OnSecondaryHeld() { }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.ReleaseSecondarySkill() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}
