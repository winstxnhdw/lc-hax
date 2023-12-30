using UnityEngine;

namespace Hax;

public sealed class StunMod : MonoBehaviour {
    Collider[] Colliders { get; set; } = new Collider[20];

    void OnEnable() {
        InputListener.onLeftButtonPress += this.Stun;
    }

    void OnDisable() {
        InputListener.onLeftButtonPress -= this.Stun;
    }

    void Stun() {
        if (!Setting.EnableStunOnLeftClick) return;
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) return;

        foreach (RaycastHit raycastHit in Helper.SphereCastForward(camera.transform)) {
            Collider collider = raycastHit.collider;

            if (collider.TryGetComponent(out EnemyAICollisionDetect enemy)) {
                enemy.mainScript.SetEnemyStunned(true, 5.0f);
                break;
            }

            if (!collider.TryGetComponent(out Turret _) && !collider.TryGetComponent(out Landmine _)) {
                continue;
            }

            if (!collider.TryGetComponent(out TerminalAccessibleObject terminalAccessibleObject)) {
                continue;
            }

            terminalAccessibleObject.CallFunctionFromTerminal();
            break;
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
