using Hax;

internal sealed class TriggerMod : MonoBehaviour, IEnemyPrompter {
    RaycastHit[] RaycastHits { get; set; } = new RaycastHit[100];

    bool UsingInteractRay { get; set; } = false;
    bool UsingFollowRay { get; set; } = false;

    void OnEnable() {
        InputListener.OnMiddleButtonPress += this.Fire;
        InputListener.OnEButtonHold += this.SetUsingInteractRay;
        InputListener.OnFButtonHold += this.SetUsingFollowRay;
    }

    void OnDisable() {
        InputListener.OnMiddleButtonPress -= this.Fire;
        InputListener.OnEButtonHold -= this.SetUsingInteractRay;
        InputListener.OnFButtonHold -= this.SetUsingFollowRay;
    }

    void SetUsingInteractRay(bool isHeld) => this.UsingInteractRay = isHeld;

    void SetUsingFollowRay(bool isHeld) => this.UsingFollowRay = isHeld;

    void Fire() {
        if (Helper.CurrentCamera is not Camera camera) return;

        if (this.UsingFollowRay) {
            if (FollowMod.PlayerToFollow is not null) {
                FollowMod.PlayerToFollow = null;
                Chat.Print("Stopped following!");
                return;
            }

            foreach (int i in this.RaycastHits.SphereCastForward(camera.transform).Range()) {
                if (!this.RaycastHits[i].collider.TryGetComponent(out PlayerControllerB player)) continue;
                if (player.IsSelf()) continue;

                Chat.Print($"Following #{player.playerClientId} {player.playerUsername}!");
                FollowMod.PlayerToFollow = player;
                break;
            }

            return;
        }

        if (this.UsingInteractRay) {
            foreach (int i in this.RaycastHits.SphereCastForward(camera.transform, 0.25f).Range()) {
                if (!this.RaycastHits[i].collider.TryGetComponent(out InteractTrigger interactTrigger)) continue;

                interactTrigger.Interact(Helper.LocalPlayer?.transform);
                break;
            }

            return;
        }

        if (HaxObjects.Instance?.DepositItemsDesk?.Object.Unfake() is DepositItemsDesk deposit) {
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

            if (collider.GetComponentInParent<EnemyAI>().Unfake() is EnemyAI enemy && Setting.EnablePhantom) {
                PossessionMod.Instance?.Possess(enemy);
                break;
            }

            if (collider.TryGetComponent(out PlayerControllerB player)) {
                Helper.GetEnemy<CentipedeAI>()?.ClingToPlayerServerRpc(player.playerClientId);
                this.PromptEnemiesToTarget(targetPlayer: player)
                    .ForEach(enemy => Chat.Print($"{enemy} prompted!"));
                break;
            }
        }
    }
}
