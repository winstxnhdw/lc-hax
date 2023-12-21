using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public sealed class TriggerMod : MonoBehaviour {
    bool UsingInteractRay { get; set; } = false;
    bool UsingFollowRay { get; set; } = false;
    bool FunnyReviveEnabled { get; set; } = false;

    DepositItemsDesk? DepositItemsDesk => HaxObjects.Instance?.DepositItemsDesk.Object;

    void OnEnable() {
        InputListener.onMiddleButtonPress += this.Fire;
        InputListener.onEButtonHold += this.SetUsingInteractRay;
        InputListener.onRButtonHold += this.SetFunnyReviveEnabled;
        InputListener.onFButtonHold += this.SetUsingFollowRay;
    }

    void OnDisable() {
        InputListener.onMiddleButtonPress -= this.Fire;
        InputListener.onEButtonHold -= this.SetUsingInteractRay;
        InputListener.onRButtonHold -= this.SetFunnyReviveEnabled;
        InputListener.onFButtonHold -= this.SetUsingFollowRay;
    }

    void SetUsingInteractRay(bool isHeld) => this.UsingInteractRay = isHeld;

    void SetUsingFollowRay(bool isHeld) => this.UsingFollowRay = isHeld;

    void SetFunnyReviveEnabled(bool isHeld) => this.FunnyReviveEnabled = isHeld;

    void Fire() {
        if (this.UsingFollowRay) {
            if (Settings.PlayerToFollow is not null) {
                Settings.PlayerToFollow = null;
                Console.Print("Stopped following!");
                return;
            }

            foreach (RaycastHit raycastHit in Helper.RaycastForward()) {
                GameObject gameObject = raycastHit.collider.gameObject;

                if (gameObject.GetComponent<PlayerControllerB>().IsNotNull(out PlayerControllerB player)) {
                    Console.Print($"Following #{player.playerClientId} {player.playerUsername}!");
                    Settings.PlayerToFollow = player;
                    break;
                }
            }

            return;
        }

        if (this.UsingInteractRay) {
            foreach (RaycastHit raycastHit in Helper.RaycastForward(0.25f)) {
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

            if (gameObject.GetComponent<TerminalAccessibleObject>().IsNotNull(out TerminalAccessibleObject terminalObject)) {
                terminalObject.SetDoorOpenServerRpc(!terminalObject.Reflect().GetInternalField<bool>("isDoorOpen"));
            }

            if (gameObject.GetComponent<Turret>().IsNotNull(out Turret turret)) {
                turret.EnterBerserkModeServerRpc(-1);
            }

            if (gameObject.GetComponent<Landmine>().IsNotNull(out Landmine landmine)) {
                landmine.TriggerMine();
                break;
            }

            if (gameObject.GetComponent<JetpackItem>().IsNotNull(out JetpackItem jetpack)) {
                jetpack.ExplodeJetpackServerRpc();
                break;
            }

            if (gameObject.GetComponent<DoorLock>().IsNotNull(out DoorLock doorLock)) {
                doorLock.UnlockDoorSyncWithServer();
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
