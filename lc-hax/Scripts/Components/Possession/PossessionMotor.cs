using System;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

sealed class PossessionMotor : IDisposable {
    EnemyAI Enemy { get; }
    IEnemyController Controller { get; }
    GameObject Root { get; }
    CharacterMovement Movement { get; }
    KeyboardMovement NoClipMovement { get; }

    internal bool NoClipEnabled { get; private set; }

    internal PossessionMotor(EnemyAI enemy, IEnemyController controller) {
        this.Enemy = enemy;
        this.Controller = controller;
        this.Root = new GameObject("Hax Possession");
        this.Root.transform.position = this.Enemy.transform.position - this.Controller.PositionOffset;
        this.Movement = this.Root.AddComponent<CharacterMovement>();
        this.Movement.enabled = false;
        this.Movement.CalibrateCollision(this.Enemy);
        this.Movement.SprintMultiplier = this.Controller.SprintMultiplier;
        this.Movement.Speed = this.GetInitialSpeed();
        this.NoClipMovement = this.Root.AddComponent<KeyboardMovement>();
        this.NoClipMovement.enabled = false;
        Object.DontDestroyOnLoad(this.Root);
    }

    float GetInitialSpeed() {
        if (this.Enemy.agent.Unfake() is { speed: > 0.0f } agent) return agent.speed;

        NavMeshAgent? prefabAgent = this.Enemy.enemyType.enemyPrefab.Unfake()?.GetComponentInChildren<NavMeshAgent>(includeInactive: true);

        return prefabAgent is { speed: > 0.0f } ? prefabAgent.speed : this.Movement.Speed;
    }

    internal void SetPosition(Vector3 position) {
        Vector3 movementPosition = position - this.Controller.PositionOffset;
        this.Movement.SetPosition(movementPosition);
        this.NoClipMovement.LastPosition = movementPosition;
    }

    internal void SetManualControl(bool isManual) {
        this.NoClipEnabled = false;
        this.NoClipMovement.enabled = false;
        this.SetPosition(this.Enemy.transform.position);
        this.Movement.CanMove = false;
        this.Movement.enabled = isManual;
    }

    internal void Suspend() {
        this.Movement.CanMove = false;
        this.Movement.enabled = false;
        this.NoClipMovement.enabled = false;
    }

    internal void ToggleNoClip() {
        this.NoClipEnabled = !this.NoClipEnabled;
        this.Movement.enabled = !this.NoClipEnabled;
        this.NoClipMovement.enabled = this.NoClipEnabled;
    }

    internal void Prepare(Quaternion lookRotation, bool inputPaused) {
        this.Movement.CanMove = !inputPaused && !this.Enemy.inSpecialAnimation && this.Controller.CanMove;
        this.Movement.Speed = this.Controller.MovementSpeed ?? this.Movement.Speed;
        this.NoClipMovement.IsPaused = !this.Movement.CanMove;
        this.Root.transform.rotation = this.NoClipEnabled
            ? lookRotation
            : Quaternion.Euler(0.0f, lookRotation.eulerAngles.y, 0.0f);
    }

    internal void Apply(Quaternion lookRotation) {
        if (this.Movement.CanMove && !this.Enemy.inSpecialAnimation && this.Controller.CanMove) {
            this.Enemy.transform.position = this.Root.transform.position + this.Controller.PositionOffset;
        }

        else {
            this.SetPosition(this.Enemy.transform.position);
        }

        if (!this.Enemy.inSpecialAnimation && this.Controller.CanRotate) {
            Vector3 angles = this.Enemy.transform.eulerAngles;
            angles.y = lookRotation.eulerAngles.y;
            this.Enemy.transform.eulerAngles = angles;
        }

        this.Controller.OnMovement(
            this.Movement.IsMoving || this.NoClipMovement.IsMoving,
            this.Movement.IsSprinting || this.NoClipMovement.IsSprinting
        );
    }

    public void Dispose() {
        this.Root.SetActive(false);
        Object.Destroy(this.Root);
    }
}
