using UnityEngine;
using UnityEngine.InputSystem;
using GameNetcodeStuff;

namespace Hax;

public class PhantomMod : MonoBehaviour {

    Camera? camera = null;
    bool toggleOn = false;

    PlayerControllerB? player = null;

    bool ghostCamMode = false;

    QuickKeyboardMoveAround? kInstance = null;
    QuickMouseCameraLookAround? mInstance = null;

    Vector3 ogLocalPos;
    Quaternion ogLocalRot;
    Transform? ogParent;

    void Toggle() {
        this.Log("GHOST", $"Toggling");
        this.camera = Helper.GetCurrentCamera();

        if (this.camera == null
            || !this.camera.enabled) {
            return;
        }

        PlayerControllerB? player = this.player;
        if (player == null || this.ogParent == null) {
            this.player = Helper.LocalPlayer;
            this.Log("GHOST", $"trying to grab player");

            if (player == null) return;

            this.ogParent = this.camera.transform.parent;
            return;
        }

        this.toggleOn = !this.toggleOn;

        this.Log("GHOST", $"{this.toggleOn} cam={this.camera}");

        if (this.toggleOn) {
            if (this.ghostCamMode) return;

            this.Log("GHOST", $"Enable cam");
            player.enabled = false;
            this.ogLocalPos = this.camera.transform.localPosition;
            this.ogLocalRot = this.camera.transform.localRotation;
            this.camera.transform.SetParent(null, true);
            this.kInstance = this.camera.gameObject.AddComponent<QuickKeyboardMoveAround>();
            this.mInstance = this.camera.gameObject.AddComponent<QuickMouseCameraLookAround>();

            this.ghostCamMode = true;

        }
        else {
            if (!this.ghostCamMode) return;
            this.Log("GHOST", $"Disable cam");
            Destroy(this.kInstance);
            Destroy(this.mInstance);

            this.camera.transform.SetParent(this.ogParent, false);
            this.camera.transform.localPosition = this.ogLocalPos;
            this.camera.transform.localRotation = this.ogLocalRot;

            player.enabled = true;
            this.ghostCamMode = false;
        }
    }

    void Update() {
        Keyboard keyboard = Keyboard.current;

        if (keyboard.equalsKey.wasPressedThisFrame) {
            this.Toggle();
        }
    }

    private void Log(string tag, string msg) {
        try {
            Console.Print(tag, msg);

        }
        catch {

        }
    }
}
