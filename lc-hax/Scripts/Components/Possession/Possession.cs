using System;
using GameNetcodeStuff;
using UnityEngine;

sealed class Possession : IPhantomExtension {
    bool IPhantomExtension.ControlsCamera => this.Session is { IsEnded: false };
    internal EnemyAI? PossessedEnemy => this.Session is { IsEnded: false } session ? session.Enemy : null;
    internal bool IsPossessed => this.PossessedEnemy is not null;

    Camera Camera { get; }
    PossessionBinding CameraBinding { get; }

    PossessionSession? Session { get; set; }
    PossessionBinding? EnemyBinding { get; set; }
    PossessionBinding? PlayerBinding { get; set; }

    internal Possession(Camera camera) {
        this.Camera = camera;
        this.CameraBinding = this.GetBinding(camera);
        this.CameraBinding.Possession = this;
        InputListener.OnZPress += this.ReleasePossession;
        GameListener.OnShipLeave += this.OnShipLeave;
    }

    void IDisposable.Dispose() {
        InputListener.OnZPress -= this.ReleasePossession;
        GameListener.OnShipLeave -= this.OnShipLeave;
        this.Session?.Dispose();
        this.CameraBinding.Possession = null;
    }

    void IPhantomExtension.Update() => this.Session?.Tick();

    void IPhantomExtension.LateUpdate() => this.Session?.LateTick();

    internal void Possess(EnemyAI enemy) {
        if (!enemy.IsSpawned) return;
        if (enemy.isEnemyDead) return;
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (enemy is CadaverGrowthAI or DressGirlAI) return;
        if (enemy is CadaverBloomAI { hasBurst: false }) return;

        this.Session?.End(PossessionEndReason.Replaced);
        this.Session = new PossessionSession(enemy, this.Camera);
        this.Session.OnEnded += this.OnSessionEnded;
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
        this.Session?.End(PossessionEndReason.Released);
    }

    void OnSessionEnded() {
        this.Session?.OnEnded -= this.OnSessionEnded;
        this.Session = null;
        this.EnemyBinding.Unfake()?.Possession = null;
        this.PlayerBinding.Unfake()?.Possession = null;
    }

    internal bool IsManuallyControlled(EnemyAI enemy) =>
        this.Session is { IsManuallyControlled: true } session && session.Enemy == enemy;

    internal void OnPlayerCollision(Collider other) => this.Session?.OnPlayerCollision(other);

    void OnShipLeave() => this.Session?.End(PossessionEndReason.ShipLeaving);
}
