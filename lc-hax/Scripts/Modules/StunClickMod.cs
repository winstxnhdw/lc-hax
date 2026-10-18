using UnityEngine;

[Mod]
sealed class StunClickMod : MonoBehaviour {
    Collider[] Colliders { get; } = new Collider[100];
    RaycastHit[] RaycastHits { get; } = new RaycastHit[100];

    void OnEnable() => InputListener.OnLeftButtonPress += this.Stun;

    void OnDisable() => InputListener.OnLeftButtonPress -= this.Stun;

    bool IsHoldingADefensiveWeapon() =>
        Helper.LocalPlayer?.currentlyHeldObjectServer.Unfake()?.itemProperties is { isDefensiveWeapon: true };

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
        if (!Globals.EnableStunOnLeftClick) return;
        if (Helper.CurrentCamera is not { } camera) return;
        if (this.IsHoldingADefensiveWeapon()) return;

        int colliderCount = Physics.OverlapSphereNonAlloc(camera.transform.position, 5.0f, this.Colliders);
        int hitCount = this.RaycastHits.SphereCastForward(camera.transform);

        for (int i = 0; i < hitCount; i++) {
            this.StunJam(this.RaycastHits[i].collider);
        }

        for (int i = 0; i < colliderCount; i++) {
            this.StunJam(this.Colliders[i]);
        }
    }
}
