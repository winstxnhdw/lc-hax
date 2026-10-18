using System;
using GameNetcodeStuff;
using UnityEngine;

sealed class PumaController(PumaAI enemy) : IEnemyController {
    const int TreeStateIndex = 0;
    const int StalkStateIndex = 1;

    PumaAI Enemy { get; } = enemy;
    float PreviousStoppingDistance { get; } = enemy.agent.stoppingDistance;
    bool IsFrozen { get; set; }

    bool IsInTree => this.Enemy.clingingToTree || this.Enemy.clingingToTreeAnim;
    bool CanAct => !this.IsInTree && !this.Enemy.inSpecialAnimation;

    Vector3 IEnemyController.CameraOffset => new(0.0f, 3.5f, -5.0f);
    Vector3 IEnemyController.PositionOffset => Vector3.zero;
    bool IEnemyController.CanMove => this.CanAct && !this.IsFrozen;
    bool IEnemyController.CanRotate => this.CanAct && !this.IsFrozen;
    float IEnemyController.EntranceRange => this.IsInTree || this.Enemy.inSpecialAnimation ? 0.0f : 1.5f;
    float IEnemyController.SprintMultiplier => 3.0f;
    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake() is { speed: > 0.0f } agent ? agent.speed : null;
    string? IEnemyController.PrimarySkillName => null;
    string IEnemyController.SecondarySkillName => "(HOLD) Ambush freeze";

    void IEnemyController.OnPossess() {
        if (!this.IsInTree && this.Enemy.stalkingFrozen) {
            this.ReleaseFreeze();
        }
    }

    void IEnemyController.UpdateManual() {
        this.Enemy.updateDestinationIntervalB = float.MaxValue;

        if (this.IsInTree) {
            this.LeaveTree();
            return;
        }

        this.Enemy.targetPlayer = null;
        this.Enemy.movingTowardsTargetPlayer = false;
        this.Enemy.scratching = false;
        this.Enemy.creatureAnimator.SetBool("Scratching", false);
        this.SetState(PumaController.StalkStateIndex);

        if (this.IsFrozen) {
            this.Enemy.stalkingFrozen = true;
            this.Enemy.stalkingFrozenAnim = true;
            this.Enemy.inSpecialAnimation = true;
        }
    }

    void LeaveTree() {
        this.Enemy.targetPlayer = null;

        if (this.Enemy.treeState is TreeState.Idle && this.Enemy.TargetTree != null &&
            this.Enemy.StartTreeDropOnLocalClient(relocate: false)
        ) {
            this.Enemy.StartTreeDropRpc(
                this.Enemy.clingIdlePosition,
                this.Enemy.leapFromPosition,
                this.Enemy.treeClingRotation,
                relocate: false
            );
        }

        this.Enemy.RunTreeMode();
    }

    void IEnemyController.OnPrimaryPressed() { }

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) { }

    void IEnemyController.OnSecondaryHeld() {
        if (!this.CanAct) return;
        if (this.IsFrozen) return;

        this.IsFrozen = true;
        this.Enemy.targetPlayer = null;
        this.SetState(PumaController.StalkStateIndex);
        this.Enemy.stalkingFrozen = true;
        this.Enemy.stalkingFrozenAnim = true;
        this.Enemy.inSpecialAnimation = true;
        this.Enemy.growlSource.Stop();
        this.Enemy.SyncStalkingFrozenRpc(true);
    }

    void IEnemyController.CancelSecondarySkill() => this.IsFrozen = false;

    void IEnemyController.ReleaseSecondarySkill() {
        if (this.IsInTree) return;

        this.ReleaseFreeze();
    }

    void ReleaseFreeze() {
        this.IsFrozen = false;
        this.Enemy.stalkingFrozen = false;
        this.Enemy.stalkingFrozenAnim = false;
        this.Enemy.stalkingCharge = false;
        this.Enemy.inSpecialAnimation = false;
        this.Enemy.growlSource.Play();
        this.Enemy.SyncStalkingFrozenRpc(false);
    }

    void SetState(int state) {
        if (this.Enemy.currentBehaviourStateIndex == state) return;
        this.Enemy.SwitchToBehaviourState(state);
    }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) {
        if (this.Enemy.isEnemyDead) return;

        this.Enemy.updateDestinationIntervalB = 0.0f;
        this.Enemy.agent.stoppingDistance = this.PreviousStoppingDistance;

        if (this.IsInTree) {
            return;
        }

        this.ReleaseFreeze();
        this.Enemy.scratching = false;
        this.Enemy.creatureAnimator.SetBool("Scratching", false);
        this.Enemy.targetPlayer = null;
        this.SetState(PumaController.TreeStateIndex);
    }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) {
        if (this.IsInTree) return;

        this.Enemy.CalculateAnimationDirection();
    }

    void IEnemyController.OnOutsideStatusChange() {
        if (this.Enemy.currentSearch is not { inProgress: true } search) return;
        this.Enemy.StopSearch(search, true);
    }

    bool IEnemyController.RequiresServerAI() => this.IsInTree && this.Enemy.TargetTree == null;
    void IEnemyController.Tick(bool inputPaused) { }
    void IEnemyController.OnSecondaryPressed() { }

    void IDisposable.Dispose() => this.IsFrozen = false;
}
