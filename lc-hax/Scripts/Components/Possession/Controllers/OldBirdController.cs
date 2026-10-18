using System;
using GameNetcodeStuff;
using UnityEngine;

sealed class OldBirdController(RadMechAI enemy) : IEnemyController {
    const int FlightState = 2;
    const float RocketRequestTimeout = 8.0f;
    const float RocketRequestCooldown = 1.0f;
    const float FlightRequestCooldown = 2.0f;

    RadMechAI Enemy { get; } = enemy;

    Vector3 IEnemyController.PositionOffset => Vector3.zero;

    bool IEnemyController.AllowAIToChangeOwner => false;

    float RocketRequestExpiresAt { get; set; }
    float NextRocketTime { get; set; }
    float NextFlightTime { get; set; }
    bool RocketRequested => this.RocketRequestExpiresAt > 0.0f;
    bool IsInNativeAnimation => this.Enemy.inSpecialAnimation ||
        this.Enemy.inSpecialAnimationWithPlayer != null ||
        this.Enemy.currentBehaviourStateIndex == OldBirdController.FlightState;

    Vector3 IEnemyController.CameraOffset => new(0.0f, 8.0f, -9.0f);

    float? IEnemyController.MovementSpeed => 5.0f;

    float IEnemyController.SprintMultiplier => 2.0f;

    float IEnemyController.EntranceRange => 0.0f;

    bool IEnemyController.CanMove => !this.IsInNativeAnimation && !this.Enemy.attemptingGrab;

    bool IEnemyController.CanRotate => !this.IsInNativeAnimation && !this.Enemy.attemptingGrab;

    bool IEnemyController.RequiresServerAI(bool isAIControlled) => isAIControlled || this.RocketRequested;

    void IEnemyController.Tick(bool inputPaused) {
        if (!this.RocketRequested) return;
        if (inputPaused || this.IsInNativeAnimation || this.Enemy.attemptingGrab || this.Enemy.aimingGun) {
            this.CancelRocketRequest();
            return;
        }

        if (Time.unscaledTime < this.RocketRequestExpiresAt) {
            return;
        }

        this.CancelRocketRequest();
        Helper.SendNotification(title: "Possession", body: "Old Bird rocket request timed out.");
    }

    void IEnemyController.OnPrimaryPressed() {
        if (this.RocketRequested) return;
        if (Time.unscaledTime < this.NextRocketTime) return;
        if (!this.Enemy.ventAnimationFinished) return;
        if (this.IsInNativeAnimation) return;
        if (this.Enemy.attemptingGrab) return;
        if (this.Enemy.aimingGun) return;

        this.RocketRequestExpiresAt = Time.unscaledTime + OldBirdController.RocketRequestTimeout;
        this.NextRocketTime = Time.unscaledTime + OldBirdController.RocketRequestCooldown;
    }

    void IEnemyController.OnSecondaryPressed() {
        if (Time.unscaledTime < this.NextFlightTime) return;
        if (!this.Enemy.ventAnimationFinished) return;
        if (this.IsInNativeAnimation) return;
        if (this.Enemy.attemptingGrab) return;

        this.NextFlightTime = Time.unscaledTime + OldBirdController.FlightRequestCooldown;
        this.CancelRocketRequest();
        this.Enemy.SwitchToBehaviourServerRpc(OldBirdController.FlightState);
    }

    void IEnemyController.UpdateManual() {
        if (this.IsInNativeAnimation) return;
        if (this.Enemy.searchForPlayers is { inProgress: true } search) {
            this.Enemy.StopSearch(search, true);
        }
    }

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (this.Enemy.timeSinceGrabbingPlayer < 1.0f) return;
        if (!this.Enemy.attemptingGrab) return;
        if (this.Enemy.inSpecialAnimationWithPlayer != null) return;
        if (this.Enemy.currentBehaviourStateIndex == OldBirdController.FlightState) return;
        if (player.isInElevator) return;

        this.Enemy.timeSinceGrabbingPlayer = 0.0f;
        this.Enemy.GrabPlayerServerRpc(player.PlayerIndex());
    }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }

    void CancelRocketRequest() => this.RocketRequestExpiresAt = 0.0f;

    void IDisposable.Dispose() => this.CancelRocketRequest();

    string IEnemyController.PrimarySkillName => this.RocketRequested ? "Acquiring rocket target.." : "Request rocket burst";

    string IEnemyController.SecondarySkillName => "Fly";

    void IEnemyController.OnPossess() { }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) { }

    void IEnemyController.OnOutsideStatusChange() { }

    void IEnemyController.OnAIControlChanged(bool isAIControlled) { }

    void IEnemyController.OnSecondaryHeld() { }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.ReleaseSecondarySkill() { }
}
