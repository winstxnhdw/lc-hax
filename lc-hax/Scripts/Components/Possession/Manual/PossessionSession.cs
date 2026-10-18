using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

sealed class PossessionSession(PossessionTarget target) : IPossessionSession {
    PossessionTarget Target { get; } = target;
    bool IsManuallyControlled => this.Target.IsActive && this.Target.Control.IsManuallyControlled;

    void IPossessionSession.Enter(EnemyAI enemy) {
        if (this.Target.Controller.RequiresServerAI(false)) {
            this.ApplyExecution(PossessionExecution.Server, enemy);
            return;
        }

        this.ApplyExecution(enemy.IsOwner ? PossessionExecution.Manual : PossessionExecution.Suspended, enemy);
    }

    void IPossessionSession.Tick(EnemyAI enemy, PlayerControllerB player) {
        if (this.Target.IsActive && this.Target.Controller.RequiresServerAI(false)) {
            this.ApplyExecution(PossessionExecution.Server, enemy);
            _ = this.Target.Ownership.Request(NetworkManager.ServerClientId);
            this.Target.Input.ReleasePendingSecondarySkill();
            return;
        }

        if (!enemy.IsOwner) {
            this.ApplyExecution(PossessionExecution.Suspended, enemy);
        }

        if (!this.Target.Ownership.Request(player.actualClientId)) {
            return;
        }

        if (!this.Target.IsActive) {
            this.Target.Activate();
        }

        this.ApplyExecution(PossessionExecution.Manual, enemy);
        this.Target.Input.ReleasePendingSecondarySkill();
        this.Target.Controller.UpdateManual();
        this.Target.View.Prepare(PossessionInput.IsPaused);
        this.Target.Motor.Prepare(this.Target.View.Rotation, PossessionInput.IsPaused);
        this.Target.Agent.Stop(enemy);
    }

    void IPossessionSession.LateTick(EnemyAI enemy) {
        if (this.IsManuallyControlled) {
            this.Target.Motor.Apply(this.Target.View.Rotation);
            this.Target.Agent.Stop(enemy);
            this.Target.Entrances.UpdateOutsideStatus();
            this.Target.Sync.Synchronize(force: this.Target.Input.CanUseActions && this.Target.Entrances.TryUseEntrance());
        }

        this.Target.View.Update(this.Target.View.Rotation);
        Helper.LocalPlayer?.cursorTip.text = this.Target.CursorTip;
    }

    void IPossessionSession.OnPlayerCollision(Collider other) {
        if (!this.IsManuallyControlled) return;
        if (this.Target.EnemyReference is not { ventAnimationFinished: true } enemy) return;
        if (enemy.inSpecialAnimation) return;
        if (enemy.stunNormalizedTimer >= 0.0f) return;
        if (!other.TryGetComponent(out PlayerControllerB player)) return;
        if (player is not { isPlayerControlled: true, isPlayerDead: false }) return;
        if (player.IsSelf()) return;
        if (player.isInsideFactory != enemy.enemyType.isOutsideEnemy) return;
        this.Target.Controller.OnPlayerCollision(player);
    }

    void ApplyExecution(PossessionExecution execution, EnemyAI enemy) {
        if (!this.Target.Control.ChangeExecution(execution)) return;
        if (execution is PossessionExecution.Manual && enemy.currentSearch is { inProgress: true } search) {
            enemy.StopSearch(search, true);
        }

        if (execution is PossessionExecution.Manual) {
            this.Target.Motor.SetManualControl(true);
            this.Target.View.SetManualControl(true);
            this.Target.Agent.Stop(enemy);
        }

        else if (execution is PossessionExecution.Server) {
            this.Target.Agent.Restore(enemy, this.Target.Controller);
            this.Target.Motor.SetManualControl(false);
            this.Target.View.SetManualControl(false);
        }

        else {
            this.Target.Motor.Suspend();
            this.Target.View.Suspend();
            this.Target.Agent.Restore(enemy, this.Target.Controller);
        }

        this.Target.Sync.Synchronize(force: true);
        this.Target.Input.ReleaseSecondarySkill();
    }
}
