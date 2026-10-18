using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

sealed class PossessionSessionAI(PossessionTarget target) : IPossessionSession {
    const float CameraPitch = 15.0f;

    PossessionTarget Target { get; } = target;

    void IPossessionSession.Enter(EnemyAI enemy) => this.ApplyExecution(this.ResolveExecution(enemy), enemy);

    void IPossessionSession.Tick(EnemyAI enemy, PlayerControllerB player) {
        PossessionExecution execution = this.ResolveExecution(enemy);
        this.ApplyExecution(execution, enemy);
        this.Target.Input.ReleasePendingSecondarySkill();

        if (execution is PossessionExecution.Server) {
            _ = this.Target.Ownership.Request(NetworkManager.ServerClientId);
        }

        else if (!this.Target.Controller.AllowAIToChangeOwner) {
            _ = this.Target.Ownership.Request(player.actualClientId);
        }
    }

    void IPossessionSession.LateTick(EnemyAI enemy) {
        Quaternion rotation = Quaternion.Euler(PossessionSessionAI.CameraPitch, enemy.transform.eulerAngles.y, 0.0f);
        this.Target.View.Update(rotation);
        Helper.LocalPlayer?.cursorTip.text = this.Target.CursorTip;
    }

    void IPossessionSession.OnPlayerCollision(Collider other) { }

    PossessionExecution ResolveExecution(EnemyAI enemy) {
        return this.Target.Controller.RequiresServerAI(true)
            ? PossessionExecution.Server
            : this.Target.Controller.AllowAIToChangeOwner || enemy.IsOwner
            ? PossessionExecution.AI
            : PossessionExecution.Suspended;
    }

    void ApplyExecution(PossessionExecution execution, EnemyAI enemy) {
        if (!this.Target.Control.ChangeExecution(execution)) return;
        if (execution is PossessionExecution.Suspended) {
            this.Target.Motor.Suspend();
            this.Target.View.Suspend();
        }

        if (execution is PossessionExecution.AI) {
            this.Target.Agent.ResumeAI(enemy, this.Target.Controller);
        }

        else {
            this.Target.Agent.Restore(enemy, this.Target.Controller);
        }

        if (execution is not PossessionExecution.Suspended) {
            this.Target.Motor.SetManualControl(false);
            this.Target.View.SetManualControl(false);
        }

        this.Target.Input.ReleaseSecondarySkill();
        this.Target.Sync.Synchronize(force: true);
    }
}
