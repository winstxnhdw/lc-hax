using UnityEngine;

namespace Hax;

public sealed class StunMod : MonoBehaviour {
    Collider[] Colliders { get; set; } = new Collider[20];
    RaycastHit[] RaycastHits { get; set; } = new RaycastHit[5];

    void OnEnable() {
        InputListener.onLeftButtonPress += this.Stun;
    }

    void OnDisable() {
        InputListener.onLeftButtonPress -= this.Stun;
    }

    void Stun() {
        if (!Setting.EnableStunOnLeftClick) return;
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) return;

        foreach (int i in this.RaycastHits.SphereCastForward(camera.transform).Range()) {
            Collider collider = this.RaycastHits[i].collider;

            if (collider.TryGetComponent(out EnemyAICollisionDetect enemy)) {
                enemy.mainScript.SetEnemyStunned(true, 5.0f);
            }

            if (!collider.TryGetComponent(out Turret _) && !collider.TryGetComponent(out Landmine _)) {
                continue;
            }

            if (!collider.TryGetComponent(out TerminalAccessibleObject terminalAccessibleObject)) {
                continue;
            }

            terminalAccessibleObject.CallFunctionFromTerminal();
        }

        Physics.OverlapSphereNonAlloc(camera.transform.position, 5.0f, this.Colliders)
               .Range()
               .ForEach(i => {
                   Collider collider = this.Colliders[i];

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
               });
    }
}
