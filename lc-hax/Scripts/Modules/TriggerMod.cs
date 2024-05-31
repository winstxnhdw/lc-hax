using GameNetcodeStuff;
using Hax;
using UnityEngine;

internal sealed class TriggerMod : MonoBehaviour, IEnemyPrompter
{
    internal static TriggerMod? Instance { get; private set; }
    private RaycastHit[] RaycastHits { get; } = new RaycastHit[100];

    private bool UsingInteractRay { get; set; } = false;
    private bool UsingFollowRay { get; set; } = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        InputListener.OnMiddleButtonPress += Fire;
        InputListener.OnEButtonHold += SetUsingInteractRay;
        InputListener.OnFButtonHold += SetUsingFollowRay;
    }

    private void OnDisable()
    {
        InputListener.OnMiddleButtonPress -= Fire;
        InputListener.OnEButtonHold -= SetUsingInteractRay;
        InputListener.OnFButtonHold -= SetUsingFollowRay;
    }

    private void SetUsingInteractRay(bool isHeld)
    {
        UsingInteractRay = isHeld;
    }

    private void SetUsingFollowRay(bool isHeld)
    {
        UsingFollowRay = isHeld;
    }

    private void Fire()
    {
        if (Helper.CurrentCamera is not Camera camera) return;
        if (Helper.LocalPlayer is not PlayerControllerB localplayer) return;

        if (UsingFollowRay)
        {
            if (FollowMod.PlayerToFollow is not null)
            {
                FollowMod.PlayerToFollow = null;
                Chat.Print("Stopped following!");
                return;
            }

            foreach (var i in RaycastHits.SphereCastForward(camera.transform).Range())
            {
                if (!RaycastHits[i].collider.TryGetComponent(out PlayerControllerB player)) continue;
                if (player.IsSelf()) continue;

                Chat.Print($"Following {player.playerUsername}!");
                FollowMod.PlayerToFollow = player;
                break;
            }

            return;
        }

        if (UsingInteractRay)
        {
            foreach (var i in RaycastHits.SphereCastForward(camera.transform, 0.25f).Range())
            {
                if (!RaycastHits[i].collider.TryGetComponent(out InteractTrigger interactTrigger)) continue;

                interactTrigger.Interact(Helper.LocalPlayer?.transform);
                break;
            }

            return;
        }

        foreach (var raycastHit in camera.transform.SphereCastForward())
        {
            var collider = raycastHit.collider;

            if (collider.TryGetComponent(out TerminalAccessibleObject terminalObject)) terminalObject.ToggleDoor();

            if (collider.TryGetComponent(out Turret turret))
            {
                if (!Setting.EnableStunOnLeftClick)
                    turret.Berserk();
                else
                    turret.ToggleTurret();
            }

            if (collider.TryGetComponent(out SpikeRoofTrap spike))
            {
                if (!Setting.EnableStunOnLeftClick)
                    spike.Slam();
                else
                    spike.ToggleSpikes();
            }

            if (collider.TryGetComponent(out Landmine landmine))
            {
                if (!Setting.EnableStunOnLeftClick)
                    landmine.Explode();
                else
                    landmine.ToggleLandmine();
                break;
            }

            if (collider.TryGetComponent(out JetpackItem jetpack))
            {
                jetpack.ExplodeJetpackServerRpc();
                break;
            }

            if (collider.TryGetComponent(out DoorLock doorLock))
            {
                doorLock.UnlockDoorSyncWithServer();
                break;
            }

            if (collider.GetComponentInParent<EnemyAI>().Unfake() is EnemyAI enemy && Setting.EnablePhantom &&
                PossessionMod.Instance?.PossessedEnemy != enemy)
            {
                PossessionMod.Instance?.Possess(enemy);
                break;
            }

            if (collider.TryGetComponent(out PlayerControllerB player))
            {
                this.PromptEnemiesToTarget(player).ForEach(enemy => Chat.Print($"{enemy} prompted!"));
                break;
            }

            if (collider.TryGetComponent(out DepositItemsDesk depositItemDesk))
            {
                depositItemDesk.AttackPlayersServerRpc();
                break;
            }
        }
    }
}