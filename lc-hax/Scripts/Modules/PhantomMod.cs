using GameNetcodeStuff;
using Hax;
using UnityEngine;

internal sealed class PhantomMod : MonoBehaviour
{
    internal static PhantomMod? Instance { get; private set; }
    private bool IsShiftHeld { get; set; } = false;
    private bool EnabledPossession { get; set; } = false;
    private int CurrentSpectatorIndex { get; set; } = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        InputListener.OnShiftButtonHold += HoldShift;
        InputListener.OnEqualsPress += TogglePhantom;
        InputListener.OnRightArrowKeyPress += LookAtNextPlayer;
        InputListener.OnLeftArrowKeyPress += LookAtPreviousPlayer;
    }

    private void OnDisable()
    {
        InputListener.OnShiftButtonHold -= HoldShift;
        InputListener.OnEqualsPress -= TogglePhantom;
        InputListener.OnRightArrowKeyPress -= LookAtNextPlayer;
        InputListener.OnLeftArrowKeyPress -= LookAtPreviousPlayer;
    }

    private void Update()
    {
        if (PossessionMod.Instance is not PossessionMod possessionMod) return;
        if (Helper.CurrentCamera is not Camera { enabled: true } camera) return;
        if (!camera.gameObject.TryGetComponent(out KeyboardMovement keyboard)) return;
        if (!camera.gameObject.TryGetComponent(out MousePan mouse)) return;

        var isPossessed = possessionMod.IsPossessed;
        if (!Setting.EnablePhantom)
        {
            if (!EnabledPossession) return;
            possessionMod.Unpossess();
            possessionMod.enabled = false;
            EnabledPossession = false;
        }
        else if (!isPossessed)
        {
            // If neither is currently possessed
            EnabledPossession = false;
            keyboard.enabled = true;
            mouse.enabled = true;
        }
        else if (!EnabledPossession)
        {
            // Possessing for the first frame
            EnabledPossession = true;
            if (possessionMod.IsPossessed)
                possessionMod.enabled = true;

            keyboard.enabled = false;
            mouse.enabled = false;
        }

        if (!isPossessed && Setting.EnablePhantom) keyboard.IsPaused = Helper.LocalPlayer is { isTypingChat: true };
    }


    private void HoldShift(bool isHeld)
    {
        IsShiftHeld = isHeld;
    }

    private void LookAtNextPlayer()
    {
        LookAtPlayer(1);
    }

    private void LookAtPreviousPlayer()
    {
        LookAtPlayer(-1);
    }

    private void LookAtPlayer(int indexChange)
    {
        if (!Setting.EnablePhantom || Helper.CurrentCamera is not Camera camera) return;
        if (!camera.gameObject.TryGetComponent(out KeyboardMovement keyboard)) return;

        var playerCount = Helper.Players?.Length ?? 0;
        CurrentSpectatorIndex = (CurrentSpectatorIndex + indexChange) % playerCount;


        if (Helper.GetActivePlayer(CurrentSpectatorIndex) is not PlayerControllerB targetPlayer)
        {
            LookAtNextPlayer();
            return;
        }

        keyboard.LastPosition = targetPlayer.playerEye.position;
    }

    private void PhantomEnabled(PlayerControllerB player, Camera camera)
    {
        if (!camera.TryGetComponent(out KeyboardMovement keyboard))
            keyboard = camera.gameObject.AddComponent<KeyboardMovement>();

        if (!camera.TryGetComponent(out MousePan mouse)) mouse = camera.gameObject.AddComponent<MousePan>();

        keyboard.enabled = true;
        mouse.enabled = true;
    }

    private void PhantomDisabled(PlayerControllerB player, Camera camera)
    {
        if (player.gameplayCamera is not Camera gameplayCamera) return;
        if (Helper.StartOfRound is not StartOfRound round) return;
        if (HaxCamera.Instance is not HaxCamera haxCamera) return;
        if (haxCamera.HaxCamContainer is not GameObject container) return;

        if (IsShiftHeld) player.TeleportPlayer(container.transform.position);
        if (PossessionMod.Instance is PossessionMod { IsPossessed: true } possession) possession.Unpossess();
    }

    private void TogglePhantom()
    {
        SetPhantom(!Setting.EnablePhantom);
    }

    internal void SetPhantom(bool EnablePhantom)
    {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (HaxCamera.Instance is not HaxCamera haxCamera) return;
        if (haxCamera.GetCamera() is not Camera camera) return;
        Setting.EnablePhantom = EnablePhantom;
        player.enabled = !player.IsDead() || !Setting.EnablePhantom;
        player.playerBodyAnimator.enabled = !Setting.EnablePhantom;
        player.thisController.enabled = !Setting.EnablePhantom;
        player.isFreeCamera = Setting.EnablePhantom;
        if (Setting.EnablePhantom)
            PhantomEnabled(player, camera);
        else
            PhantomDisabled(player, camera);

        haxCamera.enabled = Setting.EnablePhantom;
    }
}