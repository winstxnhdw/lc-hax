using System;
using GameNetcodeStuff;
using UnityEngine;

enum KidnapperFoxState {
    HIDING,
    STALKING,
    REELING
}

sealed class KidnapperFoxController(BushWolfEnemy enemy) : IEnemyController {
    BushWolfEnemy Enemy { get; } = enemy;

    Vector3 IEnemyController.CameraOffset => new(0.0f, 2.5f, -4.0f);
    Vector3 IEnemyController.PositionOffset => Vector3.zero;
    float IEnemyController.EntranceRange => 0.0f;
    float IEnemyController.SprintMultiplier => 1.5f;
    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake() is { speed: > 0.0f } agent ? agent.speed : null;
    bool IEnemyController.AllowAIToChangeOwner => false;

    bool CanAct =>
        this.Enemy.foundSpawningPoint && !this.Enemy.inKillAnimation &&
        this.Enemy.stunNormalizedTimer <= 0.0f && this.Enemy.matingCallTimer <= 0.0f &&
        this.Enemy.timeSinceKillingPlayer >= 2.0f && this.Enemy.timeSinceTakingDamage >= 0.35f;

    bool IEnemyController.CanMove => this.CanAct &&
        (!this.Enemy.IsBehaviourState(KidnapperFoxState.REELING) || this.Enemy.dragging);

    bool IEnemyController.CanRotate => this.CanAct && !this.Enemy.IsBehaviourState(KidnapperFoxState.REELING);

    string IEnemyController.PrimarySkillName =>
        this.Enemy.IsBehaviourState(KidnapperFoxState.REELING)
            ? "Release tongue"
            : "Shoot tongue";

    string IEnemyController.SecondarySkillName => "Mating call";

    void IEnemyController.OnPossess() => this.CancelTongue();

    void IEnemyController.OnUnpossess(PossessionEndReason reason) {
        if (!this.Enemy.IsOwner) return;
        if (this.Enemy.isEnemyDead) return;

        this.CancelTongue();
        this.Enemy.SetBehaviourState(KidnapperFoxState.HIDING);
    }

    void IEnemyController.OnAIControlChanged(bool isAIControlled) {
        if (!this.Enemy.IsOwner) return;

        if (isAIControlled) {
            this.CancelTongue();
            this.Enemy.SetBehaviourState(KidnapperFoxState.HIDING);
            return;
        }

        this.CancelTongue();
    }

    void IEnemyController.UpdateManual() {
        if (!this.Enemy.foundSpawningPoint) return;
        if (this.Enemy.inKillAnimation) return;
        if (this.Enemy.IsBehaviourState(KidnapperFoxState.REELING)) return;

        this.Enemy.SetBehaviourState(KidnapperFoxState.STALKING);
        this.Enemy.targetPlayer = null;
    }

    void IEnemyController.OnPrimaryPressed() {
        if (this.Enemy.IsBehaviourState(KidnapperFoxState.REELING)) {
            this.CancelTongue();
            return;
        }

        if (!this.CanAct) {
            return;
        }

        if (Helper.GetClosestPlayer(this.Enemy, maxDistance: this.Enemy.attackDistance) is not PlayerControllerB target) {
            return;
        }

        this.Enemy.targetPlayer = target;
        this.Enemy.SwitchToBehaviourStateOnLocalClient((int)KidnapperFoxState.REELING);
        this.Enemy.SyncTargetPlayerAndAttackServerRpc(target.PlayerIndex());
    }

    void IEnemyController.OnSecondaryPressed() {
        if (!this.CanAct) return;
        if (this.Enemy.IsBehaviourState(KidnapperFoxState.REELING)) return;

        this.Enemy.timeSinceCall = Time.realtimeSinceStartup;
        this.Enemy.MatingCallServerRpc();
    }

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (!this.Enemy.foundSpawningPoint) return;
        if (this.Enemy.inKillAnimation) return;

        float nestDistance = Vector3.Distance(this.Enemy.transform.position, this.Enemy.currentHidingSpot);
        bool retaliating = this.Enemy.timeSinceTakingDamage < 2.5f && this.Enemy.lastHitByPlayer is not null && nestDistance < 16.0f;
        bool reeledToNest = nestDistance < 7.0f && this.Enemy.dragging && !this.Enemy.startedShootingTongue && this.Enemy.targetPlayer == player;

        if (retaliating || reeledToNest) {
            player.DamagePlayerRpc(100);
            this.Enemy.DoKillPlayerAnimationServerRpc(player.PlayerIndex());
        }
    }

    void CancelTongue() {
        this.Enemy.CancelReelingPlayerIn();
        this.Enemy.SetBehaviourState(KidnapperFoxState.STALKING);
        this.Enemy.targetPlayer = null;
    }

    void IEnemyController.OnOutsideStatusChange() { }
    void IEnemyController.Tick(bool inputPaused) { }
    bool IEnemyController.RequiresServerAI(bool isAIControlled) => false;
    void IEnemyController.OnSecondaryHeld() { }
    void IEnemyController.CancelSecondarySkill() { }
    void IEnemyController.ReleaseSecondarySkill() { }
    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
    void IDisposable.Dispose() { }
}
