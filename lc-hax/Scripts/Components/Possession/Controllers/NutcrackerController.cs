using System;
using GameNetcodeStuff;
using UnityEngine;

enum NutcrackerState {
    WALKING,
    SENTRY
}

sealed class NutcrackerController(NutcrackerEnemyAI enemy) : IEnemyController {
    NutcrackerEnemyAI Enemy { get; } = enemy;

    Vector3 IEnemyController.PositionOffset => Vector3.zero;

    float IEnemyController.SprintMultiplier => 2.8f;

    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake() is { speed: > 0.0f } agent ? agent.speed : null;

    bool InSentryMode { get; set; }
    float BehaviourSyncCooldownRemaining { get; set; }

    Vector3 IEnemyController.CameraOffset => new(0.0f, 2.8f, -3.0f);

    void IEnemyController.UpdateManual() {
        NutcrackerState state = this.InSentryMode ? NutcrackerState.SENTRY : NutcrackerState.WALKING;
        bool readyToShoot = this.Enemy.timeSinceFiringGun > 0.75f && !this.Enemy.reloadingGun && !this.Enemy.aimingGun;
        this.BehaviourSyncCooldownRemaining = Mathf.Max(0.0f, this.BehaviourSyncCooldownRemaining - Time.deltaTime);
        this.Enemy.timeSinceInspecting = 0.0f;

        if (this.Enemy.gun is ShotgunItem { shellsLoaded: <= 0 } && readyToShoot) {
            this.Enemy.reloadingGun = true;
            this.Enemy.ReloadGunServerRpc();
        }

        if (this.Enemy.IsBehaviourState(state) || this.BehaviourSyncCooldownRemaining > 0.0f) {
            return;
        }

        this.Enemy.SwitchToBehaviourServerRpc(unchecked((int)state));
        this.BehaviourSyncCooldownRemaining = 0.5f;
    }

    bool IEnemyController.CanRotate => true;

    bool IEnemyController.CanMove => true;

    void IEnemyController.OnPrimaryPressed() {
        if (this.Enemy.gun is not ShotgunItem { shellsLoaded: > 0 }) return;
        if (this.Enemy.reloadingGun) return;
        if (this.Enemy.aimingGun) return;
        if (this.Enemy.timeSinceFiringGun < 0.75f) return;

        this.Enemy.timeSinceFiringGun = 0.0f;
        this.Enemy.FireGunServerRpc();
    }

    void IEnemyController.OnSecondaryHeld() {
        if (this.Enemy.reloadingGun) return;

        this.Enemy.SetBehaviourState(NutcrackerState.SENTRY);
        this.InSentryMode = true;
    }

    void IEnemyController.CancelSecondarySkill() => this.InSentryMode = false;

    void IEnemyController.ReleaseSecondarySkill() => this.Enemy.SetBehaviourState(NutcrackerState.WALKING);

    string IEnemyController.PrimarySkillName => this.Enemy.gun is null ? "" : "Fire";

    string IEnemyController.SecondarySkillName => "(HOLD) Sentry mode";

    float IEnemyController.EntranceRange => 1.5f;

    void IEnemyController.OnOutsideStatusChange() {
        this.Enemy.StopSearch(this.Enemy.attackSearch, true);
        this.Enemy.StopSearch(this.Enemy.patrol, true);
    }

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (this.Enemy.timeSinceHittingPlayer < 1.0f) return;
        if (this.Enemy.reloadingGun) return;
        if (this.Enemy.aimingGun) return;

        this.Enemy.timeSinceHittingPlayer = 0.0f;
        this.Enemy.LegKickPlayerServerRpc(player.PlayerIndex());
    }

    void IEnemyController.OnPossess() { }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) { }

    void IDisposable.Dispose() { }

    void IEnemyController.Tick(bool inputPaused) { }

    bool IEnemyController.RequiresServerAI() => false;

    void IEnemyController.OnSecondaryPressed() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}
