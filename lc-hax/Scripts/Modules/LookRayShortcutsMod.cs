using System.Collections.Generic;
using UnityEngine;

namespace Hax;

public class LookRayShortcutsMod : MonoBehaviour {
    void OnEnable() {
        InputListener.onMiddleButtonPress += this.Fire;
    }

    void OnDisable() {
        InputListener.onMiddleButtonPress -= this.Fire;
    }

    void Fire() {
        if (!Helper.Extant(Helper.CurrentCamera, out Camera camera)) return;
        if (!camera.enabled) return;

        List<RaycastHit> raycastHits = [.. Physics.SphereCastAll(
            camera.transform.position,
            1f,
            camera.transform.forward,
            float.MaxValue
        )];

        raycastHits.ForEach(raycastHit => {
            GameObject gameObject = raycastHit.collider.gameObject;

            if (Helper.Extant(gameObject.GetComponentInParent<Landmine>(), out Landmine landmine)) {
                _ = Reflector.Target(landmine).InvokeInternalMethod("TriggerMineOnLocalClientByExiting");
                return;
            }

            if (Helper.Extant(gameObject.GetComponent<JetpackItem>(), out JetpackItem jetpack)) {
                jetpack.ExplodeJetpackServerRpc();
            }

            if (Helper.Extant(gameObject.GetComponent<Turret>(), out Turret turret)) {
                turret.EnterBerserkModeServerRpc(-1);
            }

            if (Helper.Extant(gameObject.GetComponent<DoorLock>(), out DoorLock doorLock)) {
                doorLock.UnlockDoorSyncWithServer();
            }

            if (Helper.Extant(gameObject.GetComponent<TerminalAccessibleObject>(), out TerminalAccessibleObject tao)) {
                bool isDoorOpen = Reflector.Target(tao).GetInternalField<bool>("isDoorOpen");
                tao.SetDoorOpenServerRpc(!isDoorOpen);
            }
        });
    }
}
