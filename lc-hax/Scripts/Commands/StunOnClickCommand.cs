using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Command("stunclick")]
sealed class StunOnClickCommand : ICommand {
    bool Enabled { get; set; }
    Collider[] Colliders { get; } = new Collider[100];
    RaycastHit[] RaycastHits { get; } = new RaycastHit[100];

    void StunJam(Collider collider) {
        if (collider.TryGetComponent(out EnemyAICollisionDetect enemy)) {
            enemy.mainScript.SetEnemyStunned(true, 5.0f);
        }

        if (!collider.TryGetComponent(out Turret _) && !collider.TryGetComponent(out Landmine _)) {
            return;
        }

        if (!collider.TryGetComponent(out TerminalAccessibleObject terminalAccessibleObject)) {
            return;
        }

        terminalAccessibleObject.CallFunctionFromTerminal();
    }

    void Stun() {
        if (Helper.CurrentCamera is not { } camera) return;
        if (Helper.LocalPlayer is not { } player) return;
        if (player.currentlyHeldObjectServer.Unfake()?.itemProperties is { isDefensiveWeapon: true }) return;

        int colliderCount = Physics.OverlapSphereNonAlloc(camera.transform.position, 5.0f, this.Colliders);
        int hitCount = this.RaycastHits.SphereCastForward(camera.transform);

        for (int i = 0; i < hitCount; i++) {
            this.StunJam(this.RaycastHits[i].collider);
        }

        for (int i = 0; i < colliderCount; i++) {
            this.StunJam(this.Colliders[i]);
        }
    }

    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        this.Enabled = !this.Enabled;

        if (this.Enabled) {
            InputListener.OnLeftButtonPress += this.Stun;
        }

        else {
            InputListener.OnLeftButtonPress -= this.Stun;
        }

        Chat.Print($"Stunclick: {(this.Enabled ? "Enabled" : "Disabled")}");
        return Task.CompletedTask;
    }
}
