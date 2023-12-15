using System.Collections.Generic;
using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class TriggerMod : MonoBehaviour {
    DepositItemsDesk? DepositItemsDesk => HaxObject.Instance?.DepositItemsDesk.Object;

    void OnEnable() {
        InputListener.onMiddleButtonPress += this.Fire;
    }

    void OnDisable() {
        InputListener.onMiddleButtonPress -= this.Fire;
    }

    void Fire() {
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) return;
        if (!camera.enabled) return;
        if (this.DepositItemsDesk.IsNotNull(out DepositItemsDesk deposit)) {
            deposit.AttackPlayersServerRpc();
            return;
        }

        List<RaycastHit> raycastHits = [.. Physics.SphereCastAll(
            camera.transform.position,
            1f,
            camera.transform.forward,
            float.MaxValue
        )];

        raycastHits.ForEach(raycastHit => {
            GameObject gameObject = raycastHit.collider.gameObject;

            if (gameObject.GetComponent<Landmine>().IsNotNull(out Landmine landmine)) {
                _ = Reflector.Target(landmine).InvokeInternalMethod("TriggerMineOnLocalClientByExiting");
                return;
            }

            if (gameObject.GetComponent<JetpackItem>().IsNotNull(out JetpackItem jetpack)) {
                jetpack.ExplodeJetpackServerRpc();
                return;
            }

            if (gameObject.GetComponent<Turret>().IsNotNull(out Turret turret)) {
                turret.EnterBerserkModeServerRpc(-1);
                return;
            }

            if (gameObject.GetComponent<DoorLock>().IsNotNull(out DoorLock doorLock)) {
                doorLock.UnlockDoorSyncWithServer();
                return;
            }

            if (gameObject.GetComponent<TerminalAccessibleObject>().IsNotNull(out TerminalAccessibleObject terminalObject)) {
                terminalObject.SetDoorOpenServerRpc(!Reflector.Target(terminalObject).GetInternalField<bool>("isDoorOpen"));
                return;
            }

            if (gameObject.GetComponent<PlayerControllerB>().IsNotNull(out PlayerControllerB player)) {
                Helper.PromptEnemiesToTarget(player)
                      .ForEach(enemy => Helper.PrintSystem($"{enemy} prompted!"));
            }
        });
    }
}
