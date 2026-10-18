using System;
using GameNetcodeStuff;
using UnityEngine;

enum BrackenState {
    SCOUTING,
    STAND,
    ANGER
}

sealed class BrackenController(FlowermanAI enemy) : IEnemyController {
    const float AngerDuration = 11.0f;

    FlowermanAI Enemy { get; } = enemy;

    Vector3 IEnemyController.PositionOffset => Vector3.zero;

    float IEnemyController.SprintMultiplier => 2.8f;

    Vector3 IEnemyController.CameraOffset => this.Enemy.IsBehaviourState(BrackenState.SCOUTING)
        ? new(0.0f, 2.0f, -3.0f)
        : new(0.0f, 2.6f, -3.2f);

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (this.Enemy.startingKillAnimationLocalClient) return;
        if (this.Enemy.carryingPlayerBody) return;

        this.Enemy.KillPlayerAnimationServerRpc(player.PlayerIndex());
        this.Enemy.startingKillAnimationLocalClient = true;
    }

    void IEnemyController.OnPrimaryPressed() {
        if (!this.Enemy.carryingPlayerBody) {
            float angerTime = Mathf.Max(this.Enemy.angerMeter, BrackenController.AngerDuration);
            this.Enemy.angerMeter = angerTime;
            this.Enemy.SwitchToBehaviourStateOnLocalClient((int)BrackenState.ANGER);
            this.Enemy.EnterAngerModeServerRpc(angerTime);
            return;
        }

        this.Enemy.DropPlayerBodyServerRpc();
    }

    void IEnemyController.OnSecondaryPressed() => this.Enemy.SetBehaviourState(BrackenState.STAND);

    void IEnemyController.ReleaseSecondarySkill() => this.Enemy.SetBehaviourState(BrackenState.SCOUTING);

    bool IEnemyController.CanMove => !this.Enemy.inKillAnimation;

    bool IEnemyController.CanRotate => !this.Enemy.inKillAnimation;

    string IEnemyController.PrimarySkillName => this.Enemy.carryingPlayerBody ? "Drop body" : "Anger";

    string IEnemyController.SecondarySkillName => "Stand";

    float IEnemyController.EntranceRange => 1.5f;

    float? IEnemyController.MovementSpeed => null;

    void IEnemyController.OnPossess() { }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) { }

    void IDisposable.Dispose() { }

    void IEnemyController.OnOutsideStatusChange() { }

    void IEnemyController.Tick(bool inputPaused) { }

    void IEnemyController.UpdateManual() { }

    bool IEnemyController.RequiresServerAI() => false;

    void IEnemyController.OnSecondaryHeld() { }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}
