using System;
using GameNetcodeStuff;
using UnityEngine;

sealed class BarberController(ClaySurgeonAI enemy) : IEnemyController {
    ClaySurgeonAI Enemy { get; } = enemy;
    bool StateSaved { get; set; }
    float SavedBeatTimer { get; set; }
    float NextBeatTime { get; set; }

    Vector3 IEnemyController.CameraOffset => new(0.0f, 2.5f, -3.5f);
    Vector3 IEnemyController.PositionOffset => Vector3.zero;
    bool IEnemyController.CanMove => true;
    bool IEnemyController.CanRotate => true;
    float IEnemyController.EntranceRange => 1.5f;
    float IEnemyController.SprintMultiplier => 1.0f;
    float? IEnemyController.MovementSpeed => this.Enemy.jumpSpeed;
    string IEnemyController.PrimarySkillName => "Snip";
    string? IEnemyController.SecondarySkillName => this.Enemy.isMaster ? "Dance beat" : null;
    bool IEnemyController.AllowAIToChangeOwner => false;
    bool IEnemyController.RequiresServerAI(bool isAIControlled) => false;

    void IEnemyController.OnPossess() => this.BeginManualControl();

    void BeginManualControl() {
        if (this.StateSaved) return;
        if (!this.Enemy.IsOwner) return;

        this.StateSaved = true;
        this.SavedBeatTimer = this.Enemy.beatTimer;
    }

    void IEnemyController.UpdateManual() {
        this.BeginManualControl();
        this.Enemy.hasLOS = false;
        this.Enemy.targetPlayer = null;

        if (this.Enemy.isMaster) {
            this.Enemy.beatTimer = float.MaxValue;
        }
    }

    void IEnemyController.OnPrimaryPressed() {
        this.Enemy.timeSinceSnip = 0.0f;
        this.Enemy.KillPlayerServerRpc();
    }

    void IEnemyController.OnSecondaryPressed() {
        if (!this.Enemy.isMaster) return;
        if (Time.time < this.NextBeatTime) return;

        this.Enemy.DoBeatOnOwnerClient();
        this.Enemy.beatTimer = float.MaxValue;
        this.NextBeatTime = Time.time + this.Enemy.currentInterval;
    }

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (this.Enemy.timeSinceSnip < 1.0f) return;

        this.Enemy.timeSinceSnip = 0.0f;
        player.DamagePlayerFromOtherClientServerRpc(100, Vector3.up * 14.0f, -1);
        this.Enemy.KillPlayerServerRpc();
    }

    void StopSearch() {
        if (this.Enemy.searchRoutine is not { inProgress: true } search) return;

        this.Enemy.StopSearch(search, true);
    }

    void IEnemyController.OnOutsideStatusChange() => this.StopSearch();

    void IEnemyController.OnAIControlChanged(bool isAIControlled) {
        if (!isAIControlled) {
            this.BeginManualControl();
            return;
        }

        this.RestoreNativeState();
    }

    void RestoreNativeState() {
        if (!this.StateSaved) return;

        this.Enemy.beatTimer = this.SavedBeatTimer;
        this.Enemy.updateDestinationInterval = 0.0f;
        this.StateSaved = false;
    }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) {
        if (reason is PossessionEndReason.EnemyDied) return;

        this.RestoreNativeState();
    }

    void IDisposable.Dispose() => this.StateSaved = false;

    void IEnemyController.Tick(bool inputPaused) { }

    void IEnemyController.OnSecondaryHeld() { }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.ReleaseSecondarySkill() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}
