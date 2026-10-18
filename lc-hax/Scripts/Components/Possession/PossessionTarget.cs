using System;
using UnityEngine;

enum PossessionLifecycle {
    Acquiring,
    Active,
    Ended
}

sealed class PossessionTarget : IDisposable {
    internal event Action? OnEnded;

    PossessionLifecycle Phase { get; set; }

    internal EnemyAI EnemyReference { get; }
    internal EnemyAI? Enemy => this.EnemyReference.Unfake();
    internal IEnemyController Controller { get; }
    internal PossessionMotor Motor { get; }
    internal PossessionView View { get; }
    internal PossessionOwnership Ownership { get; }
    internal PossessionAgent Agent { get; }
    internal PossessionControl Control { get; }
    internal PossessionInput Input { get; }
    internal PossessionEntrances Entrances { get; }
    internal PossessionSync Sync { get; }
    internal bool IsActive => this.Phase is PossessionLifecycle.Active;
    internal bool IsEnded => this.Phase is PossessionLifecycle.Ended;

    internal string CursorTip {
        get {
            string? primary = this.Controller.PrimarySkillName;
            string? secondary = this.Controller.SecondarySkillName;
            string separator = string.IsNullOrEmpty(primary) || string.IsNullOrEmpty(secondary) ? "" : "\n";
            return string.Concat(primary, separator, secondary);
        }
    }

    internal PossessionTarget(EnemyAI enemy, Camera camera) {
        this.EnemyReference = enemy;
        this.Ownership = new PossessionOwnership(enemy);
        this.Agent = new PossessionAgent(enemy);
        this.Controller = EnemyControllerFactory.Create(enemy);
        this.Motor = new PossessionMotor(enemy, this.Controller);
        this.View = new PossessionView(enemy, this.Controller, camera);
        this.Control = new PossessionControl(enemy, this.Controller);
        this.Input = new PossessionInput(enemy, this.Controller, this.Control);
        this.Entrances = new PossessionEntrances(enemy, this.Controller, this.Motor);
        this.Sync = new PossessionSync(enemy);
        this.Input.OnNoClipRequested += this.ToggleNoClip;
        this.Input.OnEndRequested += this.End;
    }

    internal void Activate() {
        this.Phase = PossessionLifecycle.Active;
        this.Controller.OnPossess();
        this.EnemyReference.EnableEnemyMesh(true, false);
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

        Helper.LocalPlayer?.cursorTip.text = "";
        this.Ownership.Restore(wasActive && this.Controller.AllowAIToChangeOwner);
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
