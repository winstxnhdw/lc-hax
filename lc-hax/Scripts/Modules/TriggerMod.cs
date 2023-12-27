using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public sealed class TriggerMod : MonoBehaviour, IEnemyPrompter {
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

                if (gameObject.TryGetComponent(out PlayerControllerB player)) {
                    Console.Print($"Following #{player.playerClientId} {player.playerUsername}!");
                    Settings.PlayerToFollow = player;
                    break;
                }
            }

            return;
        }

        if (this.UsingInteractRay) {
            foreach (RaycastHit raycastHit in Helper.RaycastForward(0.25f)) {
                if (!raycastHit.collider.gameObject.TryGetComponent(out InteractTrigger interactTrigger)) {
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

            if (gameObject.TryGetComponent(out TerminalAccessibleObject terminalObject)) {
                terminalObject.SetDoorOpenServerRpc(!terminalObject.Reflect().GetInternalField<bool>("isDoorOpen"));
            }

            if (gameObject.TryGetComponent(out Turret turret)) {
                turret.EnterBerserkModeServerRpc(-1);
            }

            if (gameObject.TryGetComponent(out Landmine landmine)) {
                landmine.TriggerMine();
                break;
            }

            if (gameObject.TryGetComponent(out JetpackItem jetpack)) {
                jetpack.ExplodeJetpackServerRpc();
                break;
            }

            if (gameObject.TryGetComponent(out DoorLock doorLock)) {
                doorLock.UnlockDoorSyncWithServer();
                break;
            }

            if (gameObject.TryGetComponent(out PlayerControllerB player)) {
                this.PromptEnemiesToTarget(player, this.FunnyReviveEnabled)
                    .ForEach(enemy => Console.Print($"{enemy} prompted!"));
                break;
            }

            if (gameObject.GetComponentInParent<EnemyAI>().IsNotNull(out EnemyAI enemy) &&
                Settings.PossessionMod.IsNotNull(out PossessionMod possessionMod)) {
                possessionMod.PossessEnemy(enemy);
                break;
            }
        }
    }
}
