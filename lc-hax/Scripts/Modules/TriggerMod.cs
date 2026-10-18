using GameNetcodeStuff;
using UnityEngine;

sealed class TriggerMod : MonoBehaviour, IEnemyPrompter {
    bool UsingInteractRay { get; set; }

    void OnEnable() {
        InputListener.OnMiddleButtonPress += this.Fire;
        InputListener.OnEButtonHold += this.SetUsingInteractRay;
    }

    void OnDisable() {
        InputListener.OnMiddleButtonPress -= this.Fire;
        InputListener.OnEButtonHold -= this.SetUsingInteractRay;
    }

    void SetUsingInteractRay(bool isHeld) => this.UsingInteractRay = isHeld;

    void Fire() {
        if (Helper.LocalPlayer is { isTypingChat: true } or { inTerminalMenu: true }) return;
        if (Helper.LocalPlayer?.quickMenuManager is { isMenuOpen: true }) return;
        if (Helper.CurrentCamera is not { } camera) return;

        if (this.UsingInteractRay) {
            foreach (RaycastHit hit in camera.transform.SphereCastForward(sphereRadius: 0.25f)) {
                if (!hit.collider.TryGetComponent(out InteractTrigger interactTrigger)) continue;

                interactTrigger.Interact(Helper.LocalPlayer?.transform);
                break;
            }

            return;
        }

        foreach (RaycastHit hit in camera.transform.SphereCastForward()) {
            Collider collider = hit.collider;

            if (collider.TryGetComponent(out Turret turret)) {
                turret.EnterBerserkModeServerRpc(-1);
                break;
            }

            if (collider.TryGetComponent(out Landmine landmine)) {
                landmine.TriggerMine();
                break;
            }

            if (collider.TryGetComponent(out JetpackItem jetpack)) {
                jetpack.ExplodeJetpackServerRpc();
                break;
            }

            if (collider.GetDoor() is { } doorLock) {
                doorLock.UnlockDoorSyncWithServer();
                break;
            }

            if (collider.GetComponentInParent<EnemyAI>().Unfake() is EnemyAI enemy &&
                camera.TryGetComponent(out PossessionBinding binding) &&
                binding.Possession is Possession possession
            ) {
                possession.Possess(enemy);
                break;
            }

            if (collider.TryGetComponent(out PlayerControllerB player)) {
                Helper.GetEnemy<CentipedeAI>()?.ClingToPlayerServerRpc(player.playerClientId);
                this.PromptEnemiesToTarget(targetPlayer: player)
                    .ForEach(enemy => Chat.Print($"{enemy} prompted!"));
                break;
            }

            if (collider.TryGetComponent(out DepositItemsDesk depositItemDesk)) {
                depositItemDesk.AttackPlayersServerRpc();
                break;
            }

            if (collider.TryGetComponent(out TerminalAccessibleObject terminalObject)) {
                terminalObject.SetDoorOpenServerRpc(!terminalObject.isDoorOpen);
                break;
            }
        }
    }
}
