using System;
using GameNetcodeStuff;
using UnityEngine;

sealed class ButlerController(ButlerEnemyAI enemy) : IEnemyController {
    const int RoamingState = 0;
    const int WatchingState = 1;
    const int KnifeState = 2;
    const float StabRange = 2.5f;

    ButlerEnemyAI Enemy { get; } = enemy;

    Vector3 IEnemyController.PositionOffset => Vector3.zero;

    float IEnemyController.EntranceRange => 2.5f;

    float IEnemyController.SprintMultiplier => 2.8f;

    bool IsSweeping { get; set; }
    bool SweepAnimationActive { get; set; }
    bool HasSwept { get; set; }
    bool KnifeDrawn { get; set; } = true;
    bool? WasRunning { get; set; }

    Vector3 IEnemyController.CameraOffset => new(0.0f, 2.5f, -3.0f);

    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake() is { speed: > 0.0f } agent ? agent.speed : null;

    bool IEnemyController.CanMove => !this.Enemy.inSpecialAnimation;

    bool IEnemyController.CanRotate => !this.Enemy.inSpecialAnimation;

    void IEnemyController.OnPossess() => this.ApplyManualState();

    void IEnemyController.Tick(bool inputPaused) { }

    void IEnemyController.UpdateManual() => this.ApplyManualState();

    void ApplyManualState() {
        if (this.Enemy.inSpecialAnimation) return;

        this.StopSearches();
        this.Enemy.sweepFloorTimer = float.MaxValue;
        this.Enemy.timeSinceNoticingFirstPlayer = 0.0f;
        this.Enemy.timeSinceCheckingForMultiplePlayers = 3.0f;
        this.SetSweepingAnimation(this.IsSweeping);
        this.SetState(this.IsSweeping ? ButlerController.RoamingState : this.KnifeDrawn ? ButlerController.KnifeState : ButlerController.WatchingState);
    }

    void IEnemyController.OnPrimaryPressed() {
        if (this.Enemy.inSpecialAnimation) return;
        if (this.Enemy.timeSinceHittingPlayer < 0.5f) return;

        this.StopSweeping();
        this.KnifeDrawn = true;
        this.ApplyManualState();

        if (Helper.GetClosestPlayer(this.Enemy, maxDistance: ButlerController.StabRange) is not { } target) {
            return;
        }

        this.Enemy.timeSinceHittingPlayer = 0.0f;
        target.DamagePlayerRpc(10);
        this.Enemy.StabPlayerServerRpc(target.PlayerIndex(), false);
    }

    void IEnemyController.OnSecondaryPressed() => this.BeginSweeping();

    void IEnemyController.OnSecondaryHeld() => this.BeginSweeping();

    void BeginSweeping() {
        if (this.Enemy.inSpecialAnimation) return;

        this.IsSweeping = true;
        this.KnifeDrawn = false;
        this.ApplyManualState();
    }

    void IEnemyController.ReleaseSecondarySkill() {
        this.StopSweeping();
        this.ApplyManualState();
    }

    void IEnemyController.CancelSecondarySkill() => this.StopSweeping();

    void StopSweeping() {
        this.IsSweeping = false;
        this.KnifeDrawn = false;
    }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) {
        if (this.Enemy.inSpecialAnimation) return;

        this.ApplyManualState();
        this.SetRunning(isMoving && isSprinting);
    }

    void SetSweepingAnimation(bool sweeping) {
        this.Enemy.isSweeping = false;

        if (!sweeping) {
            this.SweepAnimationActive = false;
            this.Enemy.creatureAnimator.SetBool("Sweeping", false);
            this.Enemy.sweepingAudio.Stop();
            return;
        }

        if (this.SweepAnimationActive) {
            return;
        }

        this.SweepAnimationActive = true;
        this.HasSwept = true;
        this.Enemy.SetSweepingAnimServerRpc(true);
    }

    void SetRunning(bool running) {
        if (this.WasRunning == running) return;

        this.WasRunning = running;
        this.Enemy.SetButlerRunningServerRpc(running);
    }

    void SetState(int state) {
        if (this.Enemy.currentBehaviourStateIndex == state) return;
        this.Enemy.SwitchToBehaviourState(state);
    }

    void StopSearches() {
        if (this.Enemy.currentSearch is not { inProgress: true } search) return;
        this.Enemy.StopSearch(search, true);
    }

    void IEnemyController.OnOutsideStatusChange() => this.StopSearches();

    void IEnemyController.OnUnpossess(PossessionEndReason reason) {
        if (reason is PossessionEndReason.EnemyDied or PossessionEndReason.EnemyKilled) return;

        this.StopSweeping();
        if (this.Enemy is not { IsSpawned: true, isEnemyDead: false, inSpecialAnimation: false }) {
            return;
        }

        this.SetSweepingAnimation(false);

        if (this.Enemy.IsOwner) {
            this.SetRunning(false);
            this.StopSearches();
            this.Enemy.sweepFloorTimer = 0.0f;
            this.Enemy.timeSinceNoticingFirstPlayer = 0.0f;
            this.Enemy.timeSinceCheckingForMultiplePlayers = 3.0f;
            this.Enemy.updateDestinationInterval = 0.0f;
        }

        bool wasSweeping = this.HasSwept || this.Enemy.isSweeping || this.Enemy.creatureAnimator.GetBool("Sweeping");
        this.Enemy.SwitchToBehaviourState(wasSweeping ? ButlerController.WatchingState : ButlerController.RoamingState);
        this.WasRunning = null;
    }

    void IDisposable.Dispose() {
        this.StopSweeping();
        this.SweepAnimationActive = false;
        this.WasRunning = null;
    }

    string IEnemyController.PrimarySkillName => "Stab";

    string IEnemyController.SecondarySkillName => "(HOLD) Sweep";

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) { }

    bool IEnemyController.RequiresServerAI() => false;
}
