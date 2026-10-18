using System;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

enum PossessionLifecycle {
    Acquiring,
    Active,
    Ended
}

enum PossessionExecution {
    Suspended,
    Manual,
    Server
}

sealed class PossessionSession : IDisposable {
    internal event Action? OnEnded;

    PossessionLifecycle Phase { get; set; }
    PossessionExecution Execution { get; set; }

    EnemyAI EnemyReference { get; }
    internal EnemyAI? Enemy => this.EnemyReference.Unfake();
    IEnemyController Controller { get; }
    PossessionMotor Motor { get; }
    PossessionView View { get; }
    PossessionOwnership Ownership { get; }
    PossessionAgent Agent { get; }
    PossessionInput Input { get; }
    PossessionEntrances Entrances { get; }
    PossessionSync Sync { get; }
    bool IsActive => this.Phase is PossessionLifecycle.Active;
    internal bool IsEnded => this.Phase is PossessionLifecycle.Ended;
    internal bool IsManuallyControlled => this.IsActive && this.HasManualAuthority() && !this.Controller.RequiresServerAI();

    internal PossessionSession(EnemyAI enemy, Camera camera) {
        this.EnemyReference = enemy;
        this.Ownership = new PossessionOwnership(enemy);
        this.Agent = new PossessionAgent(enemy);
        this.Controller = EnemyControllerFactory.Create(enemy);
        this.Motor = new PossessionMotor(enemy, this.Controller);
        this.View = new PossessionView(enemy, this.Controller, camera);
        this.Input = new PossessionInput(enemy, this.Controller, this.HasManualAuthority);
        this.Entrances = new PossessionEntrances(enemy, this.Controller, this.Motor);
        this.Sync = new PossessionSync(enemy);
        this.Input.OnNoClipRequested += this.ToggleNoClip;
        this.Input.OnEndRequested += this.End;
    }

    bool HasManualAuthority() => this.Execution is PossessionExecution.Manual && this.Enemy is { IsSpawned: true, IsOwner: true, isEnemyDead: false };

    internal void Tick() {
        if (this.Enemy is not { IsSpawned: true } enemy) {
            this.End(PossessionEndReason.EnemyDespawned);
            return;
        }

        if (enemy.isEnemyDead) {
            this.End(PossessionEndReason.EnemyDied);
            return;
        }

        if (Helper.LocalPlayer is not PlayerControllerB player) {
            this.End(PossessionEndReason.Disconnected);
            return;
        }

        this.Input.Tick();
        this.Entrances.Tick();
        this.Sync.Tick();
        this.Controller.Tick(PossessionInput.IsPaused);

        if (this.IsActive && this.Controller.RequiresServerAI()) {
            this.ApplyExecution(PossessionExecution.Server, enemy);
            _ = this.Ownership.Request(NetworkManager.ServerClientId);
            this.Input.ReleasePendingSecondarySkill();
            return;
        }

        if (!enemy.IsOwner) {
            this.ApplyExecution(PossessionExecution.Suspended, enemy);
        }

        if (!this.Ownership.Request(player.actualClientId)) {
            return;
        }

        if (!this.IsActive) {
            this.Phase = PossessionLifecycle.Active;
            this.Controller.OnPossess();
            enemy.EnableEnemyMesh(true, false);
        }

        this.ApplyExecution(PossessionExecution.Manual, enemy);
        this.Input.ReleasePendingSecondarySkill();
        this.Controller.UpdateManual();
        this.View.Prepare(PossessionInput.IsPaused);
        this.Motor.Prepare(this.View.Rotation, PossessionInput.IsPaused);
        this.Agent.Stop(enemy);
    }

    internal void LateTick() {
        if (this.Enemy is not EnemyAI enemy) return;
        if (this.IsManuallyControlled) {
            this.Motor.Apply(this.View.Rotation);
            this.Agent.Stop(enemy);
            this.Entrances.UpdateOutsideStatus();
            this.Sync.Synchronize(force: this.Input.CanUseActions && this.Entrances.TryUseEntrance());
        }

        this.View.Update();
    }

    internal void OnPlayerCollision(Collider other) {
        if (!this.IsManuallyControlled) return;
        if (this.EnemyReference is not { ventAnimationFinished: true } enemy) return;
        if (enemy.inSpecialAnimation) return;
        if (enemy.stunNormalizedTimer >= 0.0f) return;
        if (!other.TryGetComponent(out PlayerControllerB player)) return;
        if (player is not { isPlayerControlled: true, isPlayerDead: false }) return;
        if (player.IsSelf()) return;
        if (player.isInsideFactory != enemy.enemyType.isOutsideEnemy) return;
        this.Controller.OnPlayerCollision(player);
    }

    void ApplyExecution(PossessionExecution execution, EnemyAI enemy) {
        if (this.Execution == execution) return;

        this.Execution = execution;

        if (execution is PossessionExecution.Manual && enemy.currentSearch is { inProgress: true } search) {
            enemy.StopSearch(search, true);
        }

        if (execution is PossessionExecution.Manual) {
            this.Motor.SetManualControl(true);
            this.View.SetManualControl(true);
            this.Agent.Stop(enemy);
        }

        else if (execution is PossessionExecution.Server) {
            this.Agent.Restore(enemy, this.Controller);
            this.Motor.SetManualControl(false);
            this.View.SetManualControl(false);
        }

        else {
            this.Motor.Suspend();
            this.View.Suspend();
            this.Agent.Restore(enemy, this.Controller);
        }

        this.Sync.Synchronize(force: true);
        this.Input.ReleaseSecondarySkill();
    }

    public void Dispose() => this.End(PossessionEndReason.Disabled);

    internal void End(PossessionEndReason reason) {
        if (this.IsEnded) return;

        bool wasActive = this.IsActive;
        this.Phase = PossessionLifecycle.Ended;
        this.Input.Dispose();

        if (wasActive && this.Enemy is { IsSpawned: true }) {
            this.Controller.OnUnpossess(reason);
        }

        if (reason is PossessionEndReason.EnemyKilled && this.Enemy is { IsSpawned: true } target) {
            Helper.Kill(target);
        }

        if (wasActive && this.Enemy is { IsSpawned: true }) {
            this.Agent.Restore(this.EnemyReference, this.Controller);
            this.Sync.Synchronize(force: true);
        }

        this.Ownership.Restore();
        this.Controller.Dispose();
        this.View.Dispose();
        this.Motor.Dispose();
        this.OnEnded?.Invoke();
    }

    void ToggleNoClip() {
        this.Motor.ToggleNoClip();
        Helper.SendNotification(
            title: "Possession",
            body: $"NoClip: {(this.Motor.NoClipEnabled ? "Enabled" : "Disabled")}"
        );
    }
}
