using System;
using GameNetcodeStuff;
using UnityEngine;

sealed class ManeaterController(CaveDwellerAI enemy) : IEnemyController {
    CaveDwellerAI Enemy { get; } = enemy;
    bool IsBaby => this.Enemy.currentBehaviourStateIndex == 0;
    bool StateSaved { get; set; }
    bool AttackRequested { get; set; }
    float SavedTrapInterval { get; set; }
    float NextKillTime { get; set; }

    Vector3 IEnemyController.CameraOffset => this.IsBaby ? new(0.0f, 1.2f, -2.0f) : new(0.0f, 2.4f, -3.5f);
    Vector3 IEnemyController.PositionOffset => Vector3.zero;
    float IEnemyController.EntranceRange => this.IsBaby ? 0.0f : 1.5f;
    float IEnemyController.SprintMultiplier => 1.0f;
    bool IEnemyController.CanMove => !this.IsBaby && !this.Enemy.inSpecialAnimation && !this.Enemy.inKillAnimation;
    bool IEnemyController.CanRotate => !this.IsBaby && !this.Enemy.inSpecialAnimation && !this.Enemy.inKillAnimation;
    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake() is { speed: > 0.0f } agent ? agent.speed : null;
    string? IEnemyController.PrimarySkillName => this.IsBaby ? null : "Scream and leap";
    string? IEnemyController.SecondarySkillName => this.IsBaby ? null : "Fake cry";

    bool IEnemyController.RequiresServerAI() => this.IsBaby;

    void IEnemyController.OnPossess() => this.BeginManualControl();

    void BeginManualControl() {
        if (this.StateSaved) return;
        if (this.IsBaby) return;
        if (!this.Enemy.IsOwner) return;

        this.StateSaved = true;
        this.SavedTrapInterval = this.Enemy.checkIfTrappedInterval;
    }

    void IEnemyController.UpdateManual() {
        if (this.IsBaby) return;

        this.BeginManualControl();

        if (!this.StateSaved) {
            return;
        }

        this.Enemy.checkIfTrappedInterval = Time.realtimeSinceStartup;

        if (this.Enemy.inSpecialAnimation || this.Enemy.inKillAnimation) {
            return;
        }

        if (this.AttackRequested &&
            this.Enemy.targetPlayer is { isPlayerControlled: true, isPlayerDead: false } &&
            !this.Enemy.chasingAfterLeap
        ) {
            return;
        }

        this.StopAttack();
    }

    void IEnemyController.OnPrimaryPressed() {
        if (this.IsBaby) return;
        if (this.Enemy.inSpecialAnimation) return;
        if (this.Enemy.inKillAnimation) return;
        if (this.AttackRequested) return;
        if (Helper.GetClosestPlayer(this.Enemy) is not { } target) return;

        this.AttackRequested = true;
        this.Enemy.SwitchToBehaviourState(3);
        this.Enemy.previousBehaviourState = 3;
        this.Enemy.targetPlayer = target;
        this.Enemy.chasingAfterLeap = false;
        this.Enemy.screaming = false;
        this.Enemy.leaping = false;
        this.Enemy.startedLeapingThisFrame = false;
        this.Enemy.screamTimer = this.Enemy.screamTime;
        this.Enemy.headRig.weight = 0.0f;
        this.Enemy.useSecondaryAudiosOnAnimatedObjects = true;
        this.Enemy.openDoorSpeedMultiplier = 1.5f;
    }

    void StopAttack() {
        this.AttackRequested = false;

        if (this.Enemy.currentBehaviourStateIndex == 1) {
            return;
        }

        this.Enemy.SwitchToBehaviourState(1);
        this.Enemy.previousBehaviourState = -1;
        this.Enemy.DoNonBabyUpdateLogic();
        this.Enemy.FinishLeapServerRpc();
    }

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (this.IsBaby) return;
        if (this.Enemy.currentBehaviourStateIndex != 3) return;
        if (this.Enemy.leapTimer < 0.02f) return;
        if (this.Enemy.inKillAnimation) return;
        if (this.Enemy.startingKillAnimationLocalClient) return;
        if (Time.time < this.NextKillTime) return;

        this.NextKillTime = Time.time + 0.5f;
        player.DamagePlayerRpc(100);
    }

    void IEnemyController.OnSecondaryPressed() {
        if (this.IsBaby) return;
        if (this.Enemy.inSpecialAnimation) return;
        if (this.Enemy.inKillAnimation) return;
        if (this.Enemy.fakeCrySFX.Length == 0) return;

        int clip = UnityEngine.Random.Range(0, this.Enemy.fakeCrySFX.Length);
        this.Enemy.DoFakeCryLocalClient(clip);
        this.Enemy.MakeFakeCryServerRpc(clip);
    }

    void StopSearch() {
        if (this.Enemy.currentSearch is not { inProgress: true } search) return;

        this.Enemy.StopSearch(search, true);
    }

    void IEnemyController.OnOutsideStatusChange() => this.StopSearch();

    void IEnemyController.OnUnpossess(PossessionEndReason reason) {
        if (reason is PossessionEndReason.EnemyDied) return;
        if (!this.StateSaved) return;
        if (this.Enemy.IsOwner && !this.IsBaby && !this.Enemy.inSpecialAnimation && !this.Enemy.inKillAnimation) {
            this.StopAttack();
        }

        this.Enemy.checkIfTrappedInterval = this.SavedTrapInterval;
        this.Enemy.updateDestinationInterval = 0.0f;
        this.StateSaved = false;
        this.AttackRequested = false;
    }

    void IDisposable.Dispose() {
        this.StateSaved = false;
        this.AttackRequested = false;
    }

    void IEnemyController.Tick(bool inputPaused) { }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.OnSecondaryHeld() { }

    void IEnemyController.ReleaseSecondarySkill() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}
