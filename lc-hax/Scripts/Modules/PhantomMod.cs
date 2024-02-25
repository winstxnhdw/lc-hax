using GameNetcodeStuff;
using Hax;
using UnityEngine;

sealed class PhantomMod : MonoBehaviour {
    bool IsShiftHeld { get; set; } = false;
    bool EnabledPossession { get; set; } = false;
    int CurrentSpectatorIndex { get; set; } = 0;

    void OnEnable() {
        InputListener.OnShiftButtonHold += this.HoldShift;
        InputListener.OnEqualsPress += this.TogglePhantom;
        InputListener.OnRightArrowKeyPress += this.LookAtNextPlayer;
        InputListener.OnLeftArrowKeyPress += this.LookAtPreviousPlayer;
    }

    void OnDisable() {
        InputListener.OnShiftButtonHold -= this.HoldShift;
        InputListener.OnEqualsPress -= this.TogglePhantom;
        InputListener.OnRightArrowKeyPress -= this.LookAtNextPlayer;
        InputListener.OnLeftArrowKeyPress -= this.LookAtPreviousPlayer;
    }

    void Update() {
        if (PossessionMod.Instance is not PossessionMod possessionMod) return;
        if (Helper.CurrentCamera is not Camera { enabled: true } camera) return;
        if (!camera.gameObject.TryGetComponent(out KeyboardMovement keyboard)) return;
        if (!camera.gameObject.TryGetComponent(out MousePan mouse)) return;
        if (!Setting.EnablePhantom) {
            if (!this.EnabledPossession) return;

            possessionMod.Unpossess();
            this.EnabledPossession = false;
            possessionMod.enabled = false;
        }

        else if (!possessionMod.IsPossessed) {
            this.EnabledPossession = false;
            possessionMod.enabled = false;

            keyboard.enabled = true;
            mouse.enabled = true;
        }

        // possessing monster for the first frame
        else if (!this.EnabledPossession) {
            this.EnabledPossession = true;
            possessionMod.enabled = true;
            keyboard.enabled = false;
            mouse.enabled = false;
        }

        if (!possessionMod.IsPossessed && Setting.EnablePhantom) {
            keyboard.IsPaused = Helper.LocalPlayer is { isTypingChat: true };
        }
    }

    void HoldShift(bool isHeld) => this.IsShiftHeld = isHeld;

    void LookAtNextPlayer() => this.LookAtPlayer(1);

    void LookAtPreviousPlayer() => this.LookAtPlayer(-1);

    void LookAtPlayer(int indexChange) {
        if (!Setting.EnablePhantom || Helper.CurrentCamera is not Camera camera) return;
        if (!camera.gameObject.TryGetComponent(out KeyboardMovement keyboard)) return;

        int playerCount = Helper.Players?.Length ?? 0;
        this.CurrentSpectatorIndex = (this.CurrentSpectatorIndex + indexChange) % playerCount;


        if (Helper.GetActivePlayer(this.CurrentSpectatorIndex) is not PlayerControllerB targetPlayer) {
            this.LookAtNextPlayer();
            return;
        }

        keyboard.LastPosition = targetPlayer.playerEye.position;
    }

    void PhantomEnabled(Camera camera) {
        if (!camera.TryGetComponent(out KeyboardMovement keyboard)) {
            keyboard = camera.gameObject.AddComponent<KeyboardMovement>();
        }

        if (!camera.TryGetComponent(out MousePan mouse)) {
            mouse = camera.gameObject.AddComponent<MousePan>();
        }

        keyboard.enabled = true;
        mouse.enabled = true;
        camera.transform.SetParent(null, true);
    }

    void PhantomDisabled(PlayerControllerB player, Camera camera) {
        if (player.cameraContainerTransform is not Transform cameraParent) return;
        if (this.IsShiftHeld) {
            player.TeleportPlayer(camera.transform.position);
        }

        if (PossessionMod.Instance is PossessionMod { IsPossessed: true } possession) {
            possession.Unpossess();
        }

        camera.transform.SetParent(cameraParent, false);
        camera.transform.localPosition = Vector3.zero;
        camera.transform.localRotation = Quaternion.identity;

        if (camera.gameObject.TryGetComponent(out KeyboardMovement keyboard)) {
            keyboard.enabled = false;
        }

        if (camera.gameObject.TryGetComponent(out MousePan mouse)) {
            mouse.enabled = false;
        }

        if (player.gameplayCamera is not Camera gameplayCamera) {
            return;
        }

        if (gameplayCamera.TryGetComponent(out KeyboardMovement gameplayKeyboard)) {
            gameplayKeyboard.enabled = false;
        }

        if (gameplayCamera.TryGetComponent(out MousePan gameplayMouse)) {
            gameplayMouse.enabled = false;
        }
    }

    void TogglePhantom() {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (Helper.CurrentCamera is not Camera camera || !camera.enabled) return;

        Setting.EnablePhantom = !Setting.EnablePhantom;
        player.enabled = !player.isPlayerDead || !Setting.EnablePhantom;
        player.playerBodyAnimator.enabled = !Setting.EnablePhantom;
        player.thisController.enabled = !Setting.EnablePhantom;
        player.isFreeCamera = Setting.EnablePhantom;

        if (Setting.EnablePhantom) {
            this.PhantomEnabled(camera);
        }

        else {
            this.PhantomDisabled(player, camera);
        }
    }
}
