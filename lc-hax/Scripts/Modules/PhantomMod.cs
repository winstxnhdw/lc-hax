using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class PhantomMod : MonoBehaviour {
    bool EnablePhantom { get; set; } = false;
    int CurrentSpectatorIndex { get; set; } = 0;

    Transform? OriginalCameraParent { get; set; }
    Vector3 OriginalCameraLocalPosition { get; set; }
    Quaternion OriginalCameraLocalRotation { get; set; }
    QuickKeyboardMoveAround? KeyboardControls { get; set; }
    QuickMouseCameraLookAround? MouseControls { get; set; }

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
            this.OriginalCameraParent = camera.transform.parent;
            return;
        }

        this.EnablePhantom = !this.EnablePhantom;
        player.enabled = !this.EnablePhantom;

        if (this.EnablePhantom) {
            this.KeyboardControls = camera.gameObject.AddComponent<QuickKeyboardMoveAround>();
            this.MouseControls = camera.gameObject.AddComponent<QuickMouseCameraLookAround>();

            this.OriginalCameraLocalPosition = camera.transform.localPosition.Copy();
            this.OriginalCameraLocalRotation = camera.transform.localRotation.Copy();
            camera.transform.SetParent(null, true);
        }

        else {
            camera.transform.SetParent(this.OriginalCameraParent, false);
            camera.transform.localPosition = this.OriginalCameraLocalPosition;
            camera.transform.localRotation = this.OriginalCameraLocalRotation;

            Destroy(this.KeyboardControls);
            Destroy(this.MouseControls);
        }
    }
}
