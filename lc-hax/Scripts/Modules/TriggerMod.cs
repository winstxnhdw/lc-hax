using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class TriggerMod : MonoBehaviour {
    bool interactEnabled = false;
    DepositItemsDesk? DepositItemsDesk => HaxObject.Instance?.DepositItemsDesk.Object;

    void OnEnable() {
        InputListener.onMiddleButtonPress += this.Fire;
        InputListener.onEButtonHold += this.InteractEnabled;
    }

    void OnDisable() {
        InputListener.onMiddleButtonPress -= this.Fire;
        InputListener.onEButtonHold -= this.InteractEnabled;
    }

    void InteractEnabled(bool isHold) {
        this.interactEnabled = isHold;
    }

    void Fire() {
        if (this.interactEnabled) {
            Helper.NarrowRaycastForward.ForEach(hit => {
                GameObject gameObject = hit.collider.gameObject;

                if (gameObject.GetComponent<InteractTrigger>().IsNotNull(out InteractTrigger interactTrigger)) {
                    interactTrigger.onInteract.Invoke(null);
                }
            });
            return;
        }

        if (this.DepositItemsDesk.IsNotNull(out DepositItemsDesk deposit)) {
            deposit.AttackPlayersServerRpc();
            return;
        }

        Helper.RaycastForward.ForEach(raycastHit => {
            GameObject gameObject = raycastHit.collider.gameObject;

            if (gameObject.GetComponent<Landmine>().IsNotNull(out Landmine landmine)) {
                landmine.TriggerMine();
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
                      .ForEach(enemy => Console.Print($"{enemy} prompted!"));
            }
        });
    }
}
