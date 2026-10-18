using System;
using GameNetcodeStuff;

sealed class PossessionInput : IDisposable {
    internal event Action? OnNoClipRequested;
    internal event Action<PossessionEndReason>? OnEndRequested;

    EnemyAI Enemy { get; }
    IEnemyController Controller { get; }
    Func<bool> HasManualAuthority { get; }
    bool SecondaryHeld { get; set; }
    bool SecondaryReleasePending { get; set; }

    internal static bool IsPaused => Helper.LocalPlayer is not PlayerControllerB player || player.IsInputPaused();
    internal bool CanUseActions => !PossessionInput.IsPaused && this.HasManualAuthority() && !this.SecondaryReleasePending;

    internal PossessionInput(EnemyAI enemy, IEnemyController controller, Func<bool> hasManualAuthority) {
        this.Enemy = enemy;
        this.Controller = controller;
        this.HasManualAuthority = hasManualAuthority;

        InputListener.OnNPress += this.RequestNoClip;
        InputListener.OnLeftButtonPress += this.UsePrimarySkill;
        InputListener.OnRightButtonPress += this.UseSecondarySkill;
        InputListener.OnRightButtonRelease += this.ReleaseSecondarySkill;
        InputListener.OnRightButtonHold += this.SetSecondaryHeld;
        InputListener.OnDelPress += this.RequestKillEnemy;
    }

    internal void Tick() {
        if (!PossessionInput.IsPaused) return;
        this.ReleaseSecondarySkill();
    }

    public void Dispose() {
        InputListener.OnNPress -= this.RequestNoClip;
        InputListener.OnLeftButtonPress -= this.UsePrimarySkill;
        InputListener.OnRightButtonPress -= this.UseSecondarySkill;
        InputListener.OnRightButtonRelease -= this.ReleaseSecondarySkill;
        InputListener.OnRightButtonHold -= this.SetSecondaryHeld;
        InputListener.OnDelPress -= this.RequestKillEnemy;

        this.ReleaseSecondarySkill();
    }

    void RequestNoClip() {
        if (!this.CanUseActions) return;
        this.OnNoClipRequested?.Invoke();
    }

    void UsePrimarySkill() {
        if (!this.CanUseActions) return;
        this.Controller.OnPrimaryPressed();
    }

    void UseSecondarySkill() {
        if (!this.CanUseActions) return;

        this.SecondaryHeld = true;
        this.Controller.OnSecondaryPressed();
    }

    void SetSecondaryHeld(bool isHeld) {
        if (!isHeld) {
            this.ReleaseSecondarySkill();
            return;
        }

        if (!this.CanUseActions) {
            return;
        }

        this.SecondaryHeld = true;
        this.Controller.OnSecondaryHeld();
    }

    internal void ReleaseSecondarySkill() {
        if (this.SecondaryHeld) {
            this.SecondaryHeld = false;
            this.SecondaryReleasePending = true;
            this.Controller.CancelSecondarySkill();
        }

        this.ReleasePendingSecondarySkill();
    }

    internal void ReleasePendingSecondarySkill() {
        if (!this.SecondaryReleasePending) return;
        if (this.Enemy.Unfake() is not { IsSpawned: true, isEnemyDead: false } enemy) {
            this.SecondaryReleasePending = false;
            return;
        }

        if (!enemy.IsOwner) {
            return;
        }

        this.SecondaryReleasePending = false;
        this.Controller.ReleaseSecondarySkill();
    }

    void RequestKillEnemy() {
        if (!this.CanUseActions) return;
        this.OnEndRequested?.Invoke(PossessionEndReason.EnemyKilled);
    }
}
