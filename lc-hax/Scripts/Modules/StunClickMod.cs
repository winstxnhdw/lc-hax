using UnityEngine;
using ZLinq;

sealed class StunClickMod : MonoBehaviour {
    Collider[] Colliders { get; set; } = new Collider[100];
    RaycastHit[] RaycastHits { get; set; } = new RaycastHit[100];

    void OnEnable() => InputListener.OnLeftButtonPress += this.Stun;

    void OnDisable() => InputListener.OnLeftButtonPress -= this.Stun;

    static bool IsHoldingADefensiveWeapon() =>
        Helper.LocalPlayer?.currentlyHeldObjectServer.itemProperties is { isDefensiveWeapon: true };

    static void StunJam(Collider collider) {
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
        if (!Setting.EnableStunOnLeftClick) return;
        if (Helper.CurrentCamera is not Camera camera) return;
        if (StunClickMod.IsHoldingADefensiveWeapon()) return;

        foreach (int i in this.RaycastHits.SphereCastForward(camera.transform).Range()) {
            Collider collider = this.RaycastHits[i].collider;
            StunClickMod.StunJam(collider);
        }

        foreach (int i in Physics.OverlapSphereNonAlloc(camera.transform.position, 5.0f, this.Colliders).Range()) {
            Collider collider = this.Colliders[i];
            StunClickMod.StunJam(collider);
        }

    }
}
