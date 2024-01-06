using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public sealed class TriggerMod : MonoBehaviour, IEnemyPrompter {
    RaycastHit[] RaycastHits { get; set; } = new RaycastHit[100];

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
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) return;

        if (this.UsingFollowRay) {
            if (FollowMod.PlayerToFollow is not null) {
                FollowMod.PlayerToFollow = null;
                Console.Print("Stopped following!");
                return;
            }

            if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) {
                return;
            }

            foreach (int i in this.RaycastHits.SphereCastForward(camera.transform).Range()) {
                if (!this.RaycastHits[i].collider.TryGetComponent(out PlayerControllerB player)) continue;
                if (player.actualClientId == localPlayer.actualClientId) continue;

                Console.Print($"Following #{player.playerClientId} {player.playerUsername}!");
                FollowMod.PlayerToFollow = player;
                break;
            }

            return;
        }

        if (this.UsingInteractRay) {
            foreach (int i in this.RaycastHits.SphereCastForward(camera.transform, 0.25f).Range()) {
                if (!this.RaycastHits[i].collider.TryGetComponent(out InteractTrigger interactTrigger)) continue;
                if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) continue;

                interactTrigger.Interact(localPlayer.transform);
                break;
            }

            return;
        }

        if (this.DepositItemsDesk.IsNotNull(out DepositItemsDesk deposit)) {
            deposit.AttackPlayersServerRpc();
            return;
        }

        foreach (RaycastHit raycastHit in camera.transform.SphereCastForward()) {
            Collider collider = raycastHit.collider;

            if (collider.TryGetComponent(out TerminalAccessibleObject terminalObject)) {
                terminalObject.SetDoorOpenServerRpc(!terminalObject.Reflect().GetInternalField<bool>("isDoorOpen"));
            }

            if (collider.TryGetComponent(out Turret turret)) {
                turret.EnterBerserkModeServerRpc(-1);
            }

            if (collider.TryGetComponent(out Landmine landmine)) {
                landmine.TriggerMine();
                break;
            }

            if (collider.TryGetComponent(out JetpackItem jetpack)) {
                jetpack.ExplodeJetpackServerRpc();
                break;
            }

            if (collider.TryGetComponent(out DoorLock doorLock)) {
                doorLock.UnlockDoorSyncWithServer();
                break;
            }

            if (collider.TryGetComponent(out PlayerControllerB player)) {
                this.PromptEnemiesToTarget(player, this.FunnyReviveEnabled)
                    .ForEach(enemy => Console.Print($"{enemy} prompted!"));
                break;
            }

            if (collider.GetComponentInParent<EnemyAI>().IsNotNull(out EnemyAI enemy) &&
                PossessionMod.Instance.IsNotNull(out PossessionMod possessionMod) &&
                Setting.EnablePhantom) {
                possessionMod.Possess(enemy);
                break;
            }
        }
    }
}
