using System;
using GameNetcodeStuff;
using UnityEngine;

sealed class Possession : IPhantomExtension {
    bool IPhantomExtension.ControlsCamera => this.Target is { IsEnded: false };
    internal EnemyAI? PossessedEnemy => this.Target is { IsEnded: false } target ? target.Enemy : null;
    internal bool IsPossessed => this.PossessedEnemy is not null;
    internal bool IsAIControlled => this.Target is { IsEnded: false } && this.Session is PossessionSessionAI;

    Camera Camera { get; }
    PossessionBinding CameraBinding { get; }

    PossessionTarget? Target { get; set; }
    IPossessionSession? Session { get; set; }
    PossessionBinding? EnemyBinding { get; set; }
    PossessionBinding? PlayerBinding { get; set; }

    internal Possession(Camera camera) {
        this.Camera = camera;
        this.CameraBinding = this.GetBinding(camera);
        this.CameraBinding.Possession = this;
        InputListener.OnZPress += this.ReleasePossession;
        InputListener.OnF9Press += this.ToggleAIControl;
        GameListener.OnShipLeave += this.OnShipLeave;
    }

    void IDisposable.Dispose() {
        InputListener.OnZPress -= this.ReleasePossession;
        InputListener.OnF9Press -= this.ToggleAIControl;
        GameListener.OnShipLeave -= this.OnShipLeave;
        this.Target?.Dispose();
        this.CameraBinding.Possession = null;
    }

    void IPhantomExtension.Update() {
        if (this.Target is not PossessionTarget target) return;
        if (target.Enemy is not { IsSpawned: true } enemy) {
            target.End(PossessionEndReason.EnemyDespawned);
            return;
        }

        if (enemy.isEnemyDead) {
            target.End(PossessionEndReason.EnemyDied);
            return;
        }

        if (Helper.LocalPlayer is not PlayerControllerB player) {
            target.End(PossessionEndReason.Disconnected);
            return;
        }

        target.Input.Tick();
        target.Entrances.Tick();
        target.Sync.Tick();
        target.Controller.Tick(PossessionInput.IsPaused);
        this.Session?.Tick(enemy, player);
    }

    void IPhantomExtension.LateUpdate() {
        if (this.Target?.Enemy is not EnemyAI enemy) return;
        this.Session?.LateTick(enemy);
    }

    internal void Possess(EnemyAI enemy) {
        if (!enemy.IsSpawned) return;
        if (enemy.isEnemyDead) return;
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (enemy is CadaverGrowthAI or DressGirlAI) return;
        if (enemy is CadaverBloomAI { hasBurst: false }) return;

        this.Target?.End(PossessionEndReason.Replaced);
        this.Target = new PossessionTarget(enemy, this.Camera);
        this.Target.OnEnded += this.OnTargetEnded;
        this.Session = new PossessionSession(this.Target);
        this.EnemyBinding = this.GetBinding(enemy);
        this.PlayerBinding = this.GetBinding(player);
        this.EnemyBinding.Possession = this;
        this.PlayerBinding.Possession = this;
    }

    PossessionBinding GetBinding(Component component) => component.TryGetComponent(out PossessionBinding binding)
        ? binding
        : component.gameObject.AddComponent<PossessionBinding>();

    void ReleasePossession() {
        if (PossessionInput.IsPaused) return;
        this.Target?.End(PossessionEndReason.Released);
    }

    void OnTargetEnded() {
        this.Target?.OnEnded -= this.OnTargetEnded;
        this.Target = null;
        this.Session = null;
        this.EnemyBinding.Unfake()?.Possession = null;
        this.PlayerBinding.Unfake()?.Possession = null;
    }

    internal bool IsManuallyControlled(EnemyAI enemy) =>
        this.Target is { IsActive: true } target && target.Control.IsManuallyControlled && target.Enemy == enemy;

    internal void OnPlayerCollision(Collider other) => this.Session?.OnPlayerCollision(other);

    void ToggleAIControl() {
        if (PossessionInput.IsPaused) return;
        if (this.Target is not { IsActive: true } target) return;
        if (target.Enemy is not { IsSpawned: true, isEnemyDead: false } enemy) return;
        if (enemy.agent == null) return;
        if (enemy.inSpecialAnimation) return;
        if (!this.IsAIControlled && !target.Input.CanUseActions) return;

        this.Session = this.IsAIControlled ? new PossessionSession(target) : new PossessionSessionAI(target);
        this.Session.Enter(enemy);
        target.Controller.OnAIControlChanged(this.IsAIControlled);

        Helper.SendNotification(
            title: "Possession",
            body: $"AI Control: {(this.IsAIControlled ? "Enabled" : "Disabled")}"
        );
    }

    void OnShipLeave() => this.Target?.End(PossessionEndReason.ShipLeaving);
}
