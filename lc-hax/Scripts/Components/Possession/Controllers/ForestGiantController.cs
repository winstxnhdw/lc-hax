using System;
using GameNetcodeStuff;
using UnityEngine;

enum GiantState {
    DEFAULT = 0,
    CHASE = 1
}

sealed class ForestGiantController(ForestGiantAI enemy) : IEnemyController {
    ForestGiantAI Enemy { get; } = enemy;

    Vector3 IEnemyController.PositionOffset => Vector3.zero;

    float IEnemyController.SprintMultiplier => 2.8f;

    string? IEnemyController.PrimarySkillName => null;

    bool IsUsingSecondarySkill { get; set; }

    Vector3 IEnemyController.CameraOffset => new(0.0f, 8.0f, -8.0f);

    void IEnemyController.UpdateManual() {
        if (this.Enemy.inEatingPlayerAnimation) return;
        if (this.IsUsingSecondarySkill) return;

        this.Enemy.SetBehaviourState(GiantState.DEFAULT);
    }

    void IEnemyController.OnSecondaryHeld() {
        if (this.IsUsingSecondarySkill) return;
        if (this.Enemy.inEatingPlayerAnimation) return;
        if ((Helper.GetClosestPlayer(this.Enemy) ?? Helper.LocalPlayer) is not { } target) return;

        this.IsUsingSecondarySkill = true;
        this.Enemy.FindAndTargetNewPlayerOnLocalClient(target);
        this.Enemy.SwitchToBehaviourStateOnLocalClient((int)GiantState.CHASE);
        this.Enemy.BeginChasingNewPlayerServerRpc(target.PlayerIndex());
    }

    void IEnemyController.CancelSecondarySkill() => this.IsUsingSecondarySkill = false;

    void IEnemyController.ReleaseSecondarySkill() {
        this.IsUsingSecondarySkill = false;

        if (this.Enemy.inEatingPlayerAnimation) {
            return;
        }

        this.Enemy.SetBehaviourState(GiantState.DEFAULT);
    }

    bool IEnemyController.CanMove => !this.Enemy.inEatingPlayerAnimation;

    bool IEnemyController.CanRotate => !this.Enemy.inEatingPlayerAnimation;

    string IEnemyController.SecondarySkillName => "(HOLD) Chase";

    float IEnemyController.EntranceRange => 0.0f;

    float? IEnemyController.MovementSpeed => null;

    void IEnemyController.OnOutsideStatusChange() {
        this.Enemy.StopSearch(this.Enemy.roamPlanet, true);
        this.Enemy.StopSearch(this.Enemy.searchForPlayers, true);
    }

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (this.Enemy.inEatingPlayerAnimation) return;
        if (this.Enemy.currentBehaviourStateIndex == 2) return;

        this.Enemy.GrabPlayerServerRpc(player.PlayerIndex());
    }

    void IEnemyController.OnPossess() { }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) => this.IsUsingSecondarySkill = false;

    void IDisposable.Dispose() => this.IsUsingSecondarySkill = false;

    void IEnemyController.Tick(bool inputPaused) { }

    bool IEnemyController.RequiresServerAI() => false;

    void IEnemyController.OnPrimaryPressed() { }

    void IEnemyController.OnSecondaryPressed() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}
