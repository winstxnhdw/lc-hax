#region

using Hax;
using UnityEngine;

#endregion

sealed class StunClickMod : MonoBehaviour {
    Collider[] Colliders { get; } = new Collider[100];
    RaycastHit[] RaycastHits { get; } = new RaycastHit[100];

    void OnEnable() => InputListener.OnLeftButtonPress += this.Stun;

    void OnDisable() => InputListener.OnLeftButtonPress -= this.Stun;

    bool IsHoldingADefensiveWeapon() => Helper.LocalPlayer?.currentlyHeldObjectServer.itemProperties is
        { isDefensiveWeapon: true };

    void StunJam(Collider collider) {
        if (collider.TryGetComponent(out EnemyAICollisionDetect enemy)) enemy.mainScript.SetEnemyStunned(true, 5.0f);

        if (!collider.TryGetComponent(out Turret _) && !collider.TryGetComponent(out Landmine _) &&
            !collider.TryGetComponent(out SpikeRoofTrap _)) return;

        if (!collider.TryGetComponent(out TerminalAccessibleObject terminalAccessibleObject)) return;

        terminalAccessibleObject.CallFunctionFromTerminal();
    }

    void Stun() {
        if (!Setting.EnableStunOnLeftClick) return;
        if (Helper.CurrentCamera is not Camera camera) return;
        if (this.IsHoldingADefensiveWeapon()) return;

        this.RaycastHits.SphereCastForward(camera.transform).Range().ForEach(i => {
            Collider? collider = this.RaycastHits[i].collider;
            this.StunJam(collider);
        });

        Physics.OverlapSphereNonAlloc(camera.transform.position, 5.0f, this.Colliders).Range().ForEach(i => {
            Collider collider = this.Colliders[i];
            this.StunJam(collider);
        });
    }
}
