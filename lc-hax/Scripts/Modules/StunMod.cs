using UnityEngine;
using Hax;

public sealed class StunMod : MonoBehaviour {
    Collider[] Colliders { get; set; } = new Collider[100];
    RaycastHit[] RaycastHits { get; set; } = new RaycastHit[100];

    void OnEnable() => InputListener.OnLeftButtonPress += this.Stun;
    void OnDisable() => InputListener.OnLeftButtonPress -= this.Stun;

    bool IsHoldingADefensiveWeapon() =>
        Helper.LocalPlayer?.currentlyHeldObjectServer.Unfake()?.itemProperties.isDefensiveWeapon is true;

    void StunHitJam(Collider collider) {
        if (collider.TryGetComponent(out EnemyAICollisionDetect enemy)) {
            if (Setting.EnableStunOnLeftClick) enemy.mainScript.SetEnemyStunned(true, 5.0f);
            if (Setting.EnableHitOnLeftClick) enemy.mainScript.HitEnemyServerRpc(Setting.ShovelHitForce, -1, false);
        }

        if (!Setting.EnableStunOnLeftClick || (!collider.TryGetComponent(out Turret _) && !collider.TryGetComponent(out Landmine _))) {
            return;
        }

        collider.GetComponent<TerminalAccessibleObject>().Unfake()?.CallFunctionFromTerminal();
    }

    void Stun() {
        if (!Setting.EnableStunOnLeftClick && !Setting.EnableHitOnLeftClick) return;
        if (Helper.CurrentCamera is not Camera camera) return;
        if (this.IsHoldingADefensiveWeapon()) return;

        this.RaycastHits.SphereCastForward(camera.transform).Range().ForEach(i => {
            Collider collider = this.RaycastHits[i].collider;
            this.StunHitJam(collider);
        });

        Physics.OverlapSphereNonAlloc(camera.transform.position, 5.0f, this.Colliders).Range().ForEach(i => {
            Collider collider = this.Colliders[i];
            this.StunHitJam(collider);
        });
    }
}
