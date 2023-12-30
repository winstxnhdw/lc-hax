using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public sealed class PhantomMod : MonoBehaviour {
    bool IsShiftHeld { get; set; } = false;
    bool EnablePhantom { get; set; } = false;
    bool EnabledPossession { get; set; } = false;
    int CurrentSpectatorIndex { get; set; } = 0;

    void OnEnable() {
        InputListener.onShiftButtonHold += this.HoldShift;
        InputListener.onEqualsPress += this.TogglePhantom;
        InputListener.onRightArrowKeyPress += this.LookAtNextPlayer;
        InputListener.onLeftArrowKeyPress += this.LookAtPreviousPlayer;
    }

    void OnDisable() {
        InputListener.onShiftButtonHold -= this.HoldShift;
        InputListener.onEqualsPress -= this.TogglePhantom;
        InputListener.onRightArrowKeyPress -= this.LookAtNextPlayer;
        InputListener.onLeftArrowKeyPress -= this.LookAtPreviousPlayer;
    }

    void Update() {
        if (!PossessionMod.Instance.IsNotNull(out PossessionMod possessionMod)) return;
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera) || Helper.Try(() => !camera.enabled)) return;
        if (!camera.gameObject.TryGetComponent(out KeyboardMovement keyboard)) return;
        if (!camera.gameObject.TryGetComponent(out MousePan mouse)) return;

        if (this.EnablePhantom) {
            //if was enabled possession before, but no longer possesing
            if (this.EnabledPossession && !possessionMod.IsPossessed) {
                this.EnabledPossession = false;
                possessionMod.enabled = false;

                keyboard.enabled = true;
                mouse.enabled = true;
            }

            if (!possessionMod.IsPossessed) {
                return;
            }

            // possessing monster in the first frame
            if (!this.EnabledPossession) {
                this.EnabledPossession = true;
                possessionMod.enabled = true;

                //turn off phantom's keyboard and mouse
                keyboard.enabled = false;
                mouse.enabled = false;
            }
        }

        else {
            if (this.EnabledPossession) {
                possessionMod.Unpossess();
                this.EnabledPossession = false;
                possessionMod.enabled = false;
            }
        }
    }

    void HoldShift(bool isHeld) => this.IsShiftHeld = isHeld;

    void LookAtNextPlayer() => this.LookAtPlayer(1);

    void LookAtPreviousPlayer() => this.LookAtPlayer(-1);

    void LookAtPlayer(int indexChange) {
        if (!this.EnablePhantom || !Helper.CurrentCamera.IsNotNull(out Camera camera)) return;

        int playerCount = Helper.Players?.Length ?? 0;
        this.CurrentSpectatorIndex = (this.CurrentSpectatorIndex + indexChange) % playerCount;

        if (!Helper.GetActivePlayer(this.CurrentSpectatorIndex).IsNotNull(out PlayerControllerB targetPlayer)) {
            this.LookAtNextPlayer();
            return;
        }

        camera.transform.position = targetPlayer.playerEye.position;
    }

    void TogglePhantom() {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB player)) return;
        if (!player.cameraContainerTransform.IsNotNull(out Transform cameraParent)) return;
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera) || !camera.enabled) return;

        GameObject cameraGameObject = camera.gameObject;
        this.EnablePhantom = !this.EnablePhantom;
        Setting.EnablePhantom = this.EnablePhantom;
        player.enabled = !this.EnablePhantom;

        if (this.EnablePhantom) {
            if (!cameraGameObject.TryGetComponent(out KeyboardMovement keyboard)) {
                keyboard = cameraGameObject.AddComponent<KeyboardMovement>();
            }

            if (!cameraGameObject.TryGetComponent(out MousePan mouse)) {
                mouse = cameraGameObject.AddComponent<MousePan>();
            }

            keyboard.enabled = true;
            mouse.enabled = true;
            camera.transform.SetParent(null, true);
        }

        else {
            if (this.IsShiftHeld) {
                player.TeleportPlayer(camera.transform.position);
            }

            camera.transform.SetParent(cameraParent, false);
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localRotation = Quaternion.identity;

            if (cameraGameObject.TryGetComponent(out KeyboardMovement keyboard)) {
                keyboard.enabled = false;
            }

            if (cameraGameObject.TryGetComponent(out MousePan mouse)) {
                mouse.enabled = false;
            }

            if (!player.gameplayCamera.IsNotNull(out Camera gameplayCam)) {
                return;
            }

            if (gameplayCam.TryGetComponent(out KeyboardMovement gameplayKeyboard)) {
                gameplayKeyboard.enabled = false;
            }

            if (gameplayCam.TryGetComponent(out MousePan gameplayMouse)) {
                gameplayMouse.enabled = false;
            }
        }
    }
}
