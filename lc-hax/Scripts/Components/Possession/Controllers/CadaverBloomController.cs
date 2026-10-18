using System;
using GameNetcodeStuff;
using UnityEngine;

sealed class CadaverBloomController(CadaverBloomAI enemy) : IEnemyController {
    const int ManualState = 2;
    const float BiteCooldown = 0.65f;

    CadaverBloomAI Enemy { get; } = enemy;
    int PreviousState { get; set; }
    bool HasManualState { get; set; }
    bool IsChomping { get; set; }

    bool CanAct => this.Enemy.hasBurst && !this.Enemy.inSpecialAnimation;
    int ManualBehaviour => this.Enemy.enemyBehaviourStates.Length > CadaverBloomController.ManualState
        ? CadaverBloomController.ManualState
        : 0;

    Vector3 IEnemyController.CameraOffset => new(0.0f, 2.5f, -3.0f);
    Vector3 IEnemyController.PositionOffset => Vector3.zero;
    bool IEnemyController.CanMove => this.CanAct;

    bool IEnemyController.CanRotate => this.CanAct;
    float IEnemyController.EntranceRange => 1.5f;
    float IEnemyController.SprintMultiplier => 2.0f;
    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake() is { speed: > 0.0f } agent ? agent.speed : null;

    string IEnemyController.PrimarySkillName => this.Enemy.hasBurst ? "Roar" : "Waiting to burst";

    string? IEnemyController.SecondarySkillName =>
        this.Enemy.hasBurst && this.ManualBehaviour == CadaverBloomController.ManualState ? "(HOLD) Chomp" : null;

    void IEnemyController.OnPossess() { }

    void IEnemyController.UpdateManual() {
        if (!this.Enemy.hasBurst) return;
        if (this.Enemy.inSpecialAnimation) return;
        if (!this.HasManualState) {
            this.PreviousState = this.Enemy.currentBehaviourStateIndex;
            this.HasManualState = true;
        }

        if (this.Enemy.currentBehaviourStateIndex != this.ManualBehaviour) {
            this.Enemy.SwitchToBehaviourState(this.ManualBehaviour);
        }

        this.Enemy.movingTowardsTargetPlayer = false;
    }

    void IEnemyController.OnPrimaryPressed() {
        if (!this.CanAct) return;

        this.Enemy.timeAtLastChestBurst = Time.realtimeSinceStartup;
        this.Enemy.chestPlantVisible = true;
        this.Enemy.creatureAnimator.SetBool("ChestVisible", true);
        this.Enemy.creatureVoice.PlayOneShot(this.Enemy.chestBurst);
        WalkieTalkie.TransmitOneShotAudio(this.Enemy.creatureVoice, this.Enemy.chestBurst);
        _ = RoundManager.PlayRandomClip(this.Enemy.creatureVoice, this.Enemy.chestPlantRoars);
        this.Enemy.ChestBurstRpc(true);
    }

    void IEnemyController.OnSecondaryHeld() {
        if (!this.CanAct) return;
        if (this.ManualBehaviour != CadaverBloomController.ManualState) return;
        if (this.IsChomping) return;

        this.IsChomping = true;
        this.SetChomping(true);
    }

    void IEnemyController.CancelSecondarySkill() => this.IsChomping = false;

    void IEnemyController.ReleaseSecondarySkill() {
        if (!this.HasManualState) return;
        this.SetChomping(false);
    }

    void SetChomping(bool chomping) {
        this.Enemy.chestPlantVisible = chomping;
        this.Enemy.creatureAnimator.SetBool("ChestVisible", chomping);
        this.Enemy.creatureAnimator.SetBool("ChestBite", chomping);

        if (chomping) {
            this.Enemy.burstSource.Play();
        }

        else {
            this.Enemy.burstSource.Stop();
        }

        this.Enemy.SyncChompingAnimRpc(chomping);
    }

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (!this.CanAct) return;
        if (Time.realtimeSinceStartup - this.Enemy.timeSinceHittingPlayer < CadaverBloomController.BiteCooldown) return;

        this.Enemy.timeSinceHittingPlayer = Time.realtimeSinceStartup;
        player.DamagePlayerRpc(30);
        this.Enemy.burstSource.PlayOneShot(this.Enemy.bitePlayer);
        WalkieTalkie.TransmitOneShotAudio(this.Enemy.burstSource, this.Enemy.bitePlayer);
        this.Enemy.SyncBitePlayerRpc();
    }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) {
        if (!this.HasManualState) return;
        if (this.Enemy.isEnemyDead) return;

        this.HasManualState = false;
        this.IsChomping = false;
        this.SetChomping(false);

        if (this.Enemy.currentBehaviourStateIndex == this.ManualBehaviour) {
            this.Enemy.SwitchToBehaviourState(this.PreviousState);
        }
    }

    void IEnemyController.OnOutsideStatusChange() => this.Enemy.StopSearch(this.Enemy.searchForPlayers, true);
    void IEnemyController.Tick(bool inputPaused) { }
    bool IEnemyController.RequiresServerAI() => false;
    void IEnemyController.OnSecondaryPressed() { }
    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }

    void IDisposable.Dispose() {
        this.IsChomping = false;
        this.HasManualState = false;
    }
}
