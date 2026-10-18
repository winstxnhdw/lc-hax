using System;
using GameNetcodeStuff;
using UnityEngine;

enum ManticoilState {
    IDLE,
    GLIDING
}

sealed class ManticoilController(DoublewingAI enemy) : IEnemyController {
    DoublewingAI Enemy { get; } = enemy;

    Vector3 IEnemyController.PositionOffset => Vector3.zero;

    float IEnemyController.EntranceRange => 2.5f;

    float IEnemyController.SprintMultiplier => 2.8f;

    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake()?.speed;

    Vector3 IEnemyController.CameraOffset => new(
        0.0f,
        Mathf.Max(2.3f, this.Enemy.Body.position.y - this.Enemy.transform.position.y + 2.3f),
        -3.5f
    );

    void IEnemyController.OnPrimaryPressed() => this.Enemy.SetBehaviourState(ManticoilState.IDLE);

    void IEnemyController.OnSecondaryPressed() => this.Enemy.SetBehaviourState(ManticoilState.GLIDING);

    string IEnemyController.PrimarySkillName => "Land";

    string IEnemyController.SecondarySkillName => "Glide";

    void IEnemyController.UpdateManual() {
        this.Enemy.agent.speed = this.Enemy.IsBehaviourState(ManticoilState.GLIDING) && this.Enemy.stunNormalizedTimer < 0.0f
            ? Mathf.Clamp(this.Enemy.agent.speed + (Time.deltaTime * 4.0f), 5.0f, 19.0f)
            : 0.0f;
    }

    bool IEnemyController.CanMove => this.Enemy.IsBehaviourState(ManticoilState.GLIDING);

    bool IEnemyController.CanRotate => this.Enemy.IsBehaviourState(ManticoilState.GLIDING);

    void IEnemyController.OnOutsideStatusChange() => this.Enemy.StopSearch(this.Enemy.roamGlide, true);

    void IEnemyController.OnPossess() { }

    void IEnemyController.OnUnpossess(PossessionEndReason reason) { }

    void IDisposable.Dispose() { }

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) { }

    void IEnemyController.Tick(bool inputPaused) { }

    bool IEnemyController.RequiresServerAI() => false;

    void IEnemyController.OnSecondaryHeld() { }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.ReleaseSecondarySkill() { }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) { }
}
