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

    void PhantomEnabled(PlayerControllerB player, Camera camera) {
        if (player.gameplayCamera is not Camera gameplayCamera) return;
        if (Helper.StartOfRound is not StartOfRound round) return;

        if (!player.IsDead()) {
            gameplayCamera.enabled = true;
            camera.transform.position = gameplayCamera.transform.position;
            camera.transform.rotation = gameplayCamera.transform.rotation;
        }
        else {
            round.spectateCamera.enabled = true;
            camera.transform.position = round.spectateCamera.transform.position;
            camera.transform.rotation = round.spectateCamera.transform.rotation;
        }


        if (!camera.TryGetComponent(out KeyboardMovement keyboard)) {
            keyboard = camera.gameObject.AddComponent<KeyboardMovement>();
        }

        if (!camera.TryGetComponent(out MousePan mouse)) {
            mouse = camera.gameObject.AddComponent<MousePan>();
        }

        keyboard.enabled = true;
        mouse.enabled = true;
    }

    void PhantomDisabled(PlayerControllerB player, Camera camera) {
        if (player.cameraContainerTransform is null) return;
        if (player.gameplayCamera is not Camera gameplayCamera) return;
        if (Helper.StartOfRound is not StartOfRound round) return;
        if (this.IsShiftHeld) {
            player.TeleportPlayer(camera.transform.position);
        }

        if (!player.IsDead()) {
            gameplayCamera.enabled = true;
        }
        else {
            round.spectateCamera.enabled = true;
        }


        if (PossessionMod.Instance is PossessionMod { IsPossessed: true } possession) {
            possession.Unpossess();
        }
    }

    void TogglePhantom() {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (HaxCamera.Instance is not HaxCamera haxCamera) return;
        if (haxCamera.GetCamera() is not Camera camera) return;
        Setting.EnablePhantom = !Setting.EnablePhantom;
        haxCamera.SetActive(Setting.EnablePhantom);

        player.enabled = !player.IsDead() || !Setting.EnablePhantom;
        player.playerBodyAnimator.enabled = !Setting.EnablePhantom;
        player.thisController.enabled = !Setting.EnablePhantom;
        player.isFreeCamera = Setting.EnablePhantom;
        if (Setting.EnablePhantom) {
            this.PhantomEnabled(player, camera);
        }
        else {
            this.PhantomDisabled(player, camera);
        }
    }
}
