using UnityEngine;

namespace Hax;

public sealed class StunMod : MonoBehaviour {
    Collider[] Colliders { get; set; } = new Collider[100];
    RaycastHit[] RaycastHits { get; set; } = new RaycastHit[100];

    void OnEnable() {
        InputListener.onLeftButtonPress += this.Stun;
    }

    void OnDisable() {
        InputListener.onLeftButtonPress -= this.Stun;
    }

    bool IsHoldingADefensiveWeapon() {
        return (Helper.LocalPlayer?.currentlyHeldObject).IsNotNull(out GrabbableObject currentItem) && (
            currentItem.itemProperties.isDefensiveWeapon ||
            currentItem.TryGetComponent(out Shovel _) ||
            currentItem.TryGetComponent(out ShotgunItem _)
        );
    }

    void StunAndJam(Collider collider) {
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
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) return;
        if (this.IsHoldingADefensiveWeapon()) return;

        this.RaycastHits.SphereCastForward(camera.transform).Range().ForEach(i => {
            Collider collider = this.RaycastHits[i].collider;
            this.StunAndJam(collider);
        });

        Physics.OverlapSphereNonAlloc(camera.transform.position, 5.0f, this.Colliders).Range().ForEach(i => {
            Collider collider = this.Colliders[i];
            this.StunAndJam(collider);
        });
    }
}
