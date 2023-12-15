using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class PhantomMod : MonoBehaviour {
    bool EnablePhantom { get; set; } = false;
    int CurrentSpectatorIndex { get; set; } = 0;

    Transform? OriginalCameraParent { get; set; }
    Vector3 OriginalCameraParentPosition { get; set; }
    Vector3 OriginalCameraLocalPosition { get; set; }
    Quaternion OriginalCameraLocalRotation { get; set; }
    void OnEnable() {
        InputListener.onEqualsPress += this.TogglePhantom;
        InputListener.onRightArrowKeyPress += () => this.LookAtPlayer(1);
        InputListener.onLeftArrowKeyPress += () => this.LookAtPlayer(-1);
    }

    void OnDisable() {
        InputListener.onEqualsPress -= this.TogglePhantom;
        InputListener.onRightArrowKeyPress -= () => this.LookAtPlayer(1);
        InputListener.onLeftArrowKeyPress -= () => this.LookAtPlayer(-1);
    }

    void LookAtPlayer(int indexChange) {
        if (!this.EnablePhantom || !Helper.CurrentCamera.IsNotNull(out Camera camera)) return;

        int playerCount = Helper.Players?.Length ?? 0;
        this.CurrentSpectatorIndex = (this.CurrentSpectatorIndex + indexChange) % playerCount;

        if (!Helper.GetPlayer(this.CurrentSpectatorIndex).IsNotNull(out PlayerControllerB targetPlayer)) {
            Helper.PrintSystem("Player not found!");
            return;
        }

        camera.transform.position = targetPlayer.playerEye.position;
        Helper.PrintSystem($"Spectating {targetPlayer.playerUsername}");
    }

    void TogglePhantom() {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB player)) return;
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera) || !camera.enabled) return;
        if (this.OriginalCameraParent is null) {
            this.OriginalCameraParent = player.cameraContainerTransform;
            this.OriginalCameraParentPosition = this.OriginalCameraParent.localPosition.Copy();
            return;
        }

        this.EnablePhantom = !this.EnablePhantom;
        player.enabled = !this.EnablePhantom;

        if (this.EnablePhantom) {
            if (!camera.gameObject.GetComponent<KeyboardMovement>().IsNotNull(out KeyboardMovement keyboard)) {
                keyboard = camera.gameObject.AddComponent<KeyboardMovement>();
            }
            keyboard.enabled = true;
            if (!camera.gameObject.GetComponent<MousePan>().IsNotNull(out MousePan mouse)) {
                mouse = camera.gameObject.AddComponent<MousePan>();
            }
            mouse.enabled = true;

            camera.transform.SetParent(null, true);
        }

        else {
            this.OriginalCameraParent.localPosition = this.OriginalCameraParentPosition;
            camera.transform.SetParent(this.OriginalCameraParent, false);
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localRotation = Quaternion.identity;

            if (camera.gameObject.GetComponent<KeyboardMovement>().IsNotNull(out KeyboardMovement keyboard)) {
                keyboard.enabled = false;
            }
            if (camera.gameObject.GetComponent<MousePan>().IsNotNull(out MousePan mouse)) {
                mouse.enabled = false;
            }
        }
    }
}
