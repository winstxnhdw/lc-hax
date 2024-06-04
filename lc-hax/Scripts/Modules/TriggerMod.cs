#region

using GameNetcodeStuff;
using Hax;
using UnityEngine;

#endregion

sealed class TriggerMod : MonoBehaviour, IEnemyPrompter {
    internal static TriggerMod? Instance { get; private set; }
    RaycastHit[] RaycastHits { get; } = new RaycastHit[100];

    bool UsingInteractRay { get; set; } = false;
    bool UsingFollowRay { get; set; } = false;

    void Awake() {
        if (Instance != null) {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    void OnEnable() {
        InputListener.OnMiddleButtonPress += this.Fire;
        InputListener.OnFButtonHold += this.SetUsingFollowRay;
    }

    void OnDisable() {
        InputListener.OnMiddleButtonPress -= this.Fire;
        InputListener.OnFButtonHold -= this.SetUsingFollowRay;
    }


    void SetUsingFollowRay(bool isHeld) => this.UsingFollowRay = isHeld;

    void Fire() {
        if (Helper.CurrentCamera is not Camera camera) return;
        if (Helper.LocalPlayer is not PlayerControllerB localplayer) return;

        if (this.UsingFollowRay) {
            if (FollowMod.PlayerToFollow is not null) {
                FollowMod.PlayerToFollow = null;
                Chat.Print("Stopped following!");
                return;
            }

            foreach (int i in this.RaycastHits.SphereCastForward(camera.transform).Range()) {
                if (!this.RaycastHits[i].collider.TryGetComponent(out PlayerControllerB player)) continue;
                if (player.IsSelf()) continue;

                Chat.Print($"Following {player.playerUsername}!");
                FollowMod.PlayerToFollow = player;
                break;
            }

            return;
        }

        foreach (RaycastHit raycastHit in camera.transform.SphereCastForward()) {
            Collider? collider = raycastHit.collider;

            if (collider.TryGetComponent(out TerminalAccessibleObject terminalObject)) terminalObject.ToggleDoor();

            if (collider.TryGetComponent(out Turret turret)) {
                if (!Setting.EnableStunOnLeftClick)
                    turret.Berserk();
                else
                    turret.ToggleTurret();
            }

            if (collider.TryGetComponent(out SpikeRoofTrap spike)) {
                if (!Setting.EnableStunOnLeftClick)
                    spike.Slam();
                else
                    spike.ToggleSpikes();
            }

            if (collider.TryGetComponent(out Landmine landmine)) {
                if (!Setting.EnableStunOnLeftClick)
                    landmine.Explode();
                else
                    landmine.ToggleLandmine();
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

            if (collider.GetComponentInParent<EnemyAI>().Unfake() is EnemyAI enemy && Setting.EnablePhantom &&
                PossessionMod.Instance?.PossessedEnemy != enemy) {
                PossessionMod.Instance?.Possess(enemy);
                break;
            }

            if (collider.TryGetComponent(out PlayerControllerB player)) {
                this.PromptEnemiesToTarget(player).ForEach(enemy => Chat.Print($"{enemy} prompted!"));
                break;
            }

            if (collider.TryGetComponent(out DepositItemsDesk depositItemDesk)) {
                depositItemDesk.AttackPlayersServerRpc();
                break;
            }
        }
    }
}
