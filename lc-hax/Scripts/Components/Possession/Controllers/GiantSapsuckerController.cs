using System;
using System.Collections.Generic;
using GameNetcodeStuff;
using UnityEngine;

enum GiantSapsuckerState {
    IDLE,
    GUARDING,
    ATTACKING
}

sealed class GiantSapsuckerController(GiantKiwiAI enemy) : IEnemyController {
    GiantKiwiAI Enemy { get; } = enemy;
    HashSet<ulong> PlayersHitThisPeck { get; } = [];
    float PreviousPeckTimer { get; set; }

    Vector3 IEnemyController.CameraOffset => new(0.0f, 6.0f, -7.0f);
    Vector3 IEnemyController.PositionOffset => Vector3.zero;
    float IEnemyController.EntranceRange => 0.0f;
    float IEnemyController.SprintMultiplier => 1.5f;

    bool CanAct => this.Enemy.birdNest != null && !this.Enemy.inKillAnimation && !this.Enemy.pryingOpenDoor &&
        !this.Enemy.inSpecialAnimation && this.Enemy.stunNormalizedTimer <= 0.0f;

    bool IEnemyController.CanMove => this.CanAct &&
        (!this.Enemy.IsBehaviourState(GiantSapsuckerState.ATTACKING) || this.Enemy.behaviourTimer <= 0.0f);

    bool IEnemyController.CanRotate => this.CanAct;
    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake() is { speed: > 0.0f } agent ? agent.speed : null;

    string IEnemyController.PrimarySkillName => this.Enemy.carryingEgg ? "Drop egg" : "Pick up egg";
    string IEnemyController.SecondarySkillName => "(HOLD) Peck";

    void IEnemyController.OnUnpossess(PossessionEndReason reason) {
        if (!this.Enemy.IsOwner) return;
        if (this.Enemy.isEnemyDead) return;

        this.StopPecking();
    }

    bool IEnemyController.RequiresServerAI() => false;

    void IEnemyController.Tick(bool inputPaused) => this.ObservePeck();

    void IEnemyController.UpdateManual() {
        if (!this.CanAct) return;
        if (this.Enemy.IsBehaviourState(GiantSapsuckerState.ATTACKING)) return;

        this.Enemy.SetBehaviourState(GiantSapsuckerState.GUARDING);
    }

    void IEnemyController.OnPrimaryPressed() {
        if (!this.CanAct) return;
        if (this.Enemy.IsBehaviourState(GiantSapsuckerState.ATTACKING)) return;
        if (this.Enemy.carryingEgg && this.Enemy.takeEggBackToNest is not null) {
            this.Enemy.DropEgg(false);
            return;
        }

        KiwiBabyItem? closestEgg = null;
        float closestDistance = float.MaxValue;

        foreach (KiwiBabyItem egg in this.Enemy.eggs) {
            if (egg == null) continue;
            if (egg.deactivated) continue;
            if (egg.isHeld) continue;
            if (egg.isHeldByEnemy) continue;
            if (!egg.NetworkObject.IsSpawned) continue;

            float distance = (egg.transform.position - this.Enemy.transform.position).sqrMagnitude;

            if (distance > 9.0f) {
                continue;
            }

            if (distance < closestDistance) {
                closestEgg = egg;
                closestDistance = distance;
            }
        }

        if (closestEgg is not null) {
            this.Enemy.PickUpEgg(closestEgg);
        }
    }

    void IEnemyController.OnSecondaryPressed() {
        if (!this.CanAct) return;
        if (this.Enemy.carryingEgg) return;
        if (Helper.LocalPlayer is not { } sender) return;
        if (Helper.GetClosestPlayer(this.Enemy) is not { } target) {
            this.StopPecking();
            return;
        }

        if (!target.TryGetComponent(out IVisibleThreat threat)) {
            this.StopPecking();
            return;
        }

        if (!ReferenceEquals(this.Enemy.watchingThreat, threat)) {
            this.Enemy.watchingThreat = threat;
            this.Enemy.attackingThreat = threat;
            this.Enemy.SyncWatchingThreatServerRpc(target.NetworkObject, sender.PlayerIndex());
        }

        if (!this.Enemy.IsBehaviourState(GiantSapsuckerState.ATTACKING)) {
            this.Enemy.Screech(enraged: true, sync: true);
            this.Enemy.SetBehaviourState(GiantSapsuckerState.ATTACKING);
        }
    }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.ReleaseSecondarySkill() => this.StopPecking();

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (!this.Enemy.attacking) return;
        if (this.Enemy.timeSinceHittingGround >= 0.2f) return;

        this.ObservePeck();

        if (this.PlayersHitThisPeck.Add(player.actualClientId)) {
            player.DamagePlayerRpc(10);
        }
    }

    void IEnemyController.OnPossess() {
        this.StopPecking();
        this.Enemy.StopSearch(this.Enemy.searchForLostEgg, true);
    }

    void StopPecking() {
        this.PlayersHitThisPeck.Clear();
        this.Enemy.SetBehaviourState(GiantSapsuckerState.GUARDING);
    }

    void ObservePeck() {
        float timer = this.Enemy.timeSinceHittingGround;

        if (timer < this.PreviousPeckTimer) {
            this.PlayersHitThisPeck.Clear();
        }

        this.PreviousPeckTimer = timer;
    }

    void IEnemyController.OnOutsideStatusChange() => this.Enemy.StopSearch(this.Enemy.searchForLostEgg, true);
    void IEnemyController.OnSecondaryHeld() { }
    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }

    void IDisposable.Dispose() => this.PlayersHitThisPeck.Clear();
}
