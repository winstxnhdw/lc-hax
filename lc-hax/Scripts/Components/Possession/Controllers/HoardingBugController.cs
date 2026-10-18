using System;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

enum HoardingBugState {
    IDLE,
    RETURNING_TO_NEST,
    CHASING_PLAYER
}

sealed class HoardingBugController(HoarderBugAI enemy) : IEnemyController {
    HoarderBugAI Enemy { get; } = enemy;

    Vector3 IEnemyController.CameraOffset => new(0.0f, 3.0f, -3.0f);

    Vector3 IEnemyController.PositionOffset => Vector3.zero;

    bool IEnemyController.CanMove => true;

    bool IEnemyController.CanRotate => true;

    float IEnemyController.SprintMultiplier => 2.8f;

    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake() is { speed: > 0.0f } agent ? agent.speed : null;

    void UseHeldItem(HoarderBugAI enemy) =>
        (enemy.heldItem?.itemGrabbableObject as ShotgunItem).Unfake()?.ShootShotgun(enemy.transform);

    void IEnemyController.UpdateManual() { }

    void GrabItem(HoarderBugAI enemy, GrabbableObject item) {
        NetworkObject networkObject = item.NetworkObject;
        enemy.sendingGrabOrDropRPC = true;
        enemy.GrabItem(networkObject);
        enemy.SwitchToBehaviourServerRpc(1);
        enemy.GrabItemServerRpc(networkObject);
    }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) { }

    void IEnemyController.OnPrimaryPressed() {
        if (this.Enemy.sendingGrabOrDropRPC) return;

        if (this.Enemy.heldItem is null && this.Enemy.FindNearbyItem() is GrabbableObject grabbable) {
            this.GrabItem(this.Enemy, grabbable);
            return;
        }

        this.UseHeldItem(this.Enemy);
    }

    void IEnemyController.OnSecondaryPressed() {
        if (this.Enemy.sendingGrabOrDropRPC) return;
        if (this.Enemy.heldItem is null) {
            if ((Helper.GetClosestPlayer(this.Enemy) ?? Helper.LocalPlayer) is not PlayerControllerB target) return;

            this.Enemy.watchingPlayer = target;
            this.Enemy.angryAtPlayer = target;
            this.Enemy.targetPlayer = target;
            this.Enemy.angryTimer = 15.0f;
            this.Enemy.SetBehaviourState(HoardingBugState.CHASING_PLAYER);
            return;
        }

        this.Enemy.DropItemAndCallDropRPC(this.Enemy.heldItem.itemGrabbableObject.NetworkObject, false);
    }

    string IEnemyController.PrimarySkillName => this.Enemy.heldItem is not null ? "Use item" : "Grab item";

    string IEnemyController.SecondarySkillName => this.Enemy.heldItem is null ? "Anger" : "Drop item";

    float IEnemyController.EntranceRange => 1.0f;

    void IEnemyController.OnOutsideStatusChange() {
        this.Enemy.StopSearch(this.Enemy.searchForItems, true);
        this.Enemy.StopSearch(this.Enemy.searchForPlayer, true);
    }

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (!this.Enemy.inChase) return;
        if (this.Enemy.timeSinceHittingPlayer < 0.5f) return;

        this.Enemy.timeSinceHittingPlayer = 0.0f;
        player.DamagePlayerRpc(30);
        this.Enemy.HitPlayerServerRpc();
    }

    void IEnemyController.OnPossess() { }

    void IDisposable.Dispose() { }

    void IEnemyController.Tick(bool inputPaused) { }

    bool IEnemyController.RequiresServerAI() => false;

    void IEnemyController.OnSecondaryHeld() { }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.ReleaseSecondarySkill() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}
