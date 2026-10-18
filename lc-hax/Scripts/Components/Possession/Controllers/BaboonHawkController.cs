using System;
using GameNetcodeStuff;
using UnityEngine;

enum BaboonState {
    SCOUTING = 0,
    RETURNING = 1,
    AGGRESSIVE = 2,
}

sealed class BaboonHawkController(BaboonBirdAI enemy) : IEnemyController {
    BaboonBirdAI Enemy { get; } = enemy;

    Vector3 IEnemyController.CameraOffset => new(0.0f, 3.0f, -3.0f);

    Vector3 IEnemyController.PositionOffset => Vector3.zero;

    bool IEnemyController.CanMove => !this.Enemy.doingKillAnimation;

    bool IEnemyController.CanRotate => !this.Enemy.doingKillAnimation;

    float IEnemyController.SprintMultiplier => 2.8f;

    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake() is { speed: > 0.0f } agent ? agent.speed : null;

    bool IEnemyController.AllowAIToChangeOwner => false;

    void IEnemyController.OnUnpossess(PossessionEndReason reason) { }

    void IEnemyController.OnPossess() { }

    void IDisposable.Dispose() { }

    void IEnemyController.OnOutsideStatusChange() => this.Enemy.StopSearch(this.Enemy.scoutingSearchRoutine, true);

    void IEnemyController.UpdateManual() { }

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (this.Enemy.timeSinceHitting < 0.5f) return;
        if (this.Enemy.doingKillAnimation) return;

        this.Enemy.timeSinceHitting = 0.0f;
        player.DamagePlayerRpc(20);

        if (player.health <= 20 && player.criticallyInjured) {
            this.Enemy.StabPlayerDeathAnimServerRpc(player.PlayerIndex());
            return;
        }

        this.Enemy.creatureAnimator.SetTrigger("Hit");
        this.Enemy.creatureSFX.PlayOneShot(this.Enemy.enemyType.audioClips[5]);
    }

    void IEnemyController.OnPrimaryPressed() {
        if (this.Enemy.heldScrap is null && this.Enemy.FindNearbyItem() is GrabbableObject grabbable) {
            this.Enemy.GrabItemAndSync(grabbable.NetworkObject);
            return;
        }

        (this.Enemy.heldScrap as ShotgunItem).Unfake()?.ShootShotgun(this.Enemy.transform);
    }

    void IEnemyController.OnSecondaryPressed() {
        if (this.Enemy.heldScrap is null) return;
        this.Enemy.DropHeldItemAndSync(sync: true);
    }

    string IEnemyController.PrimarySkillName => this.Enemy.heldScrap is ShotgunItem ? "Fire" : this.Enemy.heldScrap is null ? "Grab item" : "";

    string IEnemyController.SecondarySkillName => this.Enemy.heldScrap is null ? "" : "Drop item";

    float IEnemyController.EntranceRange => 1.5f;

    void IEnemyController.Tick(bool inputPaused) { }

    void IEnemyController.OnAIControlChanged(bool isAIControlled) { }

    bool IEnemyController.RequiresServerAI(bool isAIControlled) => false;

    void IEnemyController.OnSecondaryHeld() { }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.ReleaseSecondarySkill() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}
