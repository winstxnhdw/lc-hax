using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class TriggerMod : MonoBehaviour {
    bool InteractEnabled { get; set; } = false;
    bool FunnyReviveEnabled { get; set; } = false;
    bool FollowPlayerEnabled { get; set; } = false;

    DepositItemsDesk? DepositItemsDesk => HaxObject.Instance?.DepositItemsDesk.Object;

    void OnEnable() {
        InputListener.onMiddleButtonPress += this.Fire;
        InputListener.onEButtonHold += this.SetInteractEnabled;
        InputListener.onRButtonHold += this.SetFunnyReviveEnabled;
        InputListener.onFButtonHold += this.SetFollowPlayerEnabled;
    }

    void OnDisable() {
        InputListener.onMiddleButtonPress -= this.Fire;
        InputListener.onEButtonHold -= this.SetInteractEnabled;
        InputListener.onRButtonHold -= this.SetFunnyReviveEnabled;
        InputListener.onFButtonHold -= this.SetFollowPlayerEnabled;
    }

    void SetInteractEnabled(bool isHeld) => this.InteractEnabled = isHeld;

    void SetFunnyReviveEnabled(bool isHeld) => this.FunnyReviveEnabled = isHeld;

    void SetFollowPlayerEnabled(bool isHeld) => this.FollowPlayerEnabled = isHeld;

    void Fire() {
        if (this.FollowPlayerEnabled) {
            bool foundTarget = false;

            foreach (RaycastHit raycastHit in Helper.RaycastForward()) {
                GameObject gameObject = raycastHit.collider.gameObject;

                if (gameObject.GetComponent<PlayerControllerB>().IsNotNull(out PlayerControllerB player)) {
                    Console.Print($"Following #{player.playerClientId} {player.playerUsername}!");
                    Settings.PlayerToFollow = player;
                    foundTarget = true;
                    break;
                }
            }

            if (!foundTarget) {
                if (Settings.PlayerToFollow != null) {
                    Settings.PlayerToFollow = null;
                    Console.Print("Stopped following!");
                }
            }

            return;
        }

        if (this.InteractEnabled) {
            foreach (RaycastHit raycastHit in Helper.RaycastForward()) {
                if (!raycastHit.collider.gameObject.GetComponent<InteractTrigger>().IsNotNull(out InteractTrigger interactTrigger)) {
                    continue;
                }

                interactTrigger.onInteract.Invoke(null);
                break;
            }

            return;
        }

        if (this.DepositItemsDesk.IsNotNull(out DepositItemsDesk deposit)) {
            deposit.AttackPlayersServerRpc();
            return;
        }

        foreach (RaycastHit raycastHit in Helper.RaycastForward()) {
            GameObject gameObject = raycastHit.collider.gameObject;

            if (gameObject.GetComponent<Landmine>().IsNotNull(out Landmine landmine)) {
                landmine.TriggerMine();
                break;
            }

            if (gameObject.GetComponent<JetpackItem>().IsNotNull(out JetpackItem jetpack)) {
                jetpack.ExplodeJetpackServerRpc();
                break;
            }

            if (gameObject.GetComponent<Turret>().IsNotNull(out Turret turret)) {
                turret.EnterBerserkModeServerRpc(-1);
                break;
            }

            if (gameObject.GetComponent<DoorLock>().IsNotNull(out DoorLock doorLock)) {
                doorLock.UnlockDoorSyncWithServer();
                break;
            }
            if (gameObject.GetComponent<TerminalAccessibleObject>().IsNotNull(out TerminalAccessibleObject terminalObject)) {
                terminalObject.SetDoorOpenServerRpc(!Reflector.Target(terminalObject).GetInternalField<bool>("isDoorOpen"));
                break;
            }

            if (gameObject.GetComponent<PlayerControllerB>().IsNotNull(out PlayerControllerB player)) {
                Helper.PromptEnemiesToTarget(player, this.FunnyReviveEnabled)
                      .ForEach(enemy => Console.Print($"{enemy} prompted!"));
                break;
            }
        }
    }
}
