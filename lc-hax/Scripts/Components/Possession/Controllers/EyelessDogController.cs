using System;
using GameNetcodeStuff;
using UnityEngine;

enum DogState {
    ROAMING,
    SUSPICIOUS,
    CHASE,
    LUNGE
}

sealed class EyelessDogController(MouthDogAI enemy) : IEnemyController {
    const float ChaseInterval = 3.0f;

    MouthDogAI Enemy { get; } = enemy;

    Vector3 IEnemyController.PositionOffset => Vector3.zero;

    float IEnemyController.EntranceRange => 2.5f;

    float IEnemyController.SprintMultiplier => 2.8f;

    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake()?.speed;

    Vector3 IEnemyController.CameraOffset => new(0.0f, 3.2f, -4.0f);

    void IEnemyController.OnPrimaryPressed() {
        if (this.Enemy.inKillAnimation) return;
        if (this.Enemy.IsBehaviourState(DogState.LUNGE)) return;
        if (this.Enemy.IsBehaviourState(DogState.CHASE)) {
            this.Enemy.suspicionLevel = 0;
            this.Enemy.AITimer = EyelessDogController.ChaseInterval;
            this.Enemy.SetBehaviourState(DogState.SUSPICIOUS);
            return;
        }

        this.StartChase(this.Enemy.noisePositionGuess);
    }

    void StartChase(Vector3 targetPosition) {
        this.Enemy.AITimer = EyelessDogController.ChaseInterval;
        this.Enemy.EnrageDogOnLocalClient(
            targetPosition,
            Vector3.Distance(this.Enemy.transform.position, targetPosition),
            approximatePosition: false,
            fullyEnrage: true
        );
    }

    void IEnemyController.OnSecondaryPressed() => this.TryLunge();

    void TryLunge() {
        if (this.Enemy.inKillAnimation) return;
        if (!this.Enemy.IsBehaviourState(DogState.CHASE)) return;
        if (!this.Enemy.hasEnteredChaseModeFully) return;
        if (this.Enemy.inLunge) return;
        this.Enemy.inLunge = true;
        this.Enemy.EnterLunge();
    }

    bool IEnemyController.CanMove => !this.Enemy.inKillAnimation;

    bool IEnemyController.CanRotate => !this.Enemy.inKillAnimation;

    string IEnemyController.PrimarySkillName => this.Enemy.IsBehaviourState(DogState.CHASE) ? "Roam" : "Chase";

    string? IEnemyController.SecondarySkillName =>
        this.Enemy.IsBehaviourState(DogState.CHASE) && this.Enemy.hasEnteredChaseModeFully ? "Lunge" : null;

    void IEnemyController.OnOutsideStatusChange() => this.Enemy.StopSearch(this.Enemy.roamPlanet, true);

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (this.Enemy.inKillAnimation) return;

        switch ((DogState)this.Enemy.currentBehaviourStateIndex) {
            case DogState.LUNGE:
                this.Enemy.KillPlayerServerRpc(player.PlayerIndex());
                break;

            case DogState.CHASE:
                this.TryLunge();
                break;

            case DogState.ROAMING:
            case DogState.SUSPICIOUS:
                this.StartChase(player.transform.position);
                break;

            default:
                break;
        }
    }

    void IEnemyController.OnPossess() { }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) { }

    void IDisposable.Dispose() { }

    void IEnemyController.Tick(bool inputPaused) { }

    void IEnemyController.UpdateManual() {
        if (!this.Enemy.IsBehaviourState(DogState.CHASE)) return;

        this.Enemy.suspicionLevel = 11;
        this.Enemy.AITimer = EyelessDogController.ChaseInterval;
    }

    bool IEnemyController.RequiresServerAI() => false;

    void IEnemyController.OnSecondaryHeld() { }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.ReleaseSecondarySkill() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}
