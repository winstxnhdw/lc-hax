using UnityEngine;
using UnityEngine.InputSystem;
using GameNetcodeStuff;

namespace Hax;

public class PhantomMod : MonoBehaviour {

    bool toggleOn = false;
    bool ghostCamMode = false;

    QuickKeyboardMoveAround? kInstance = null;
    QuickMouseCameraLookAround? mInstance = null;

    Vector3 ogLocalPos;
    Quaternion ogLocalRot;
    Transform? ogParent;

    void Toggle() {
        if (!Helper.Extant(Helper.CurrentCamera, out Camera camera)) return;
        if (!camera.enabled) return;
        if (!Helper.Extant(Helper.LocalPlayer, out PlayerControllerB player)) return;

        if (this.ogParent == null) {
            this.ogParent = camera.transform.parent;
            return;
        }

        this.toggleOn = !this.toggleOn;

        this.Log("GHOST", $"{this.toggleOn} cam={camera}");

        if (this.toggleOn) {
            if (this.ghostCamMode) return;

            this.Log("GHOST", $"Enable cam");
            player.enabled = false;
            this.ogLocalPos = camera.transform.localPosition;
            this.ogLocalRot = camera.transform.localRotation;
            camera.transform.SetParent(null, true);
            this.kInstance = camera.gameObject.AddComponent<QuickKeyboardMoveAround>();
            this.mInstance = camera.gameObject.AddComponent<QuickMouseCameraLookAround>();

            this.ghostCamMode = true;

        }

        else {
            if (!this.ghostCamMode) return;

            Destroy(this.kInstance);
            Destroy(this.mInstance);

            camera.transform.SetParent(this.ogParent, false);
            camera.transform.localPosition = this.ogLocalPos;
            camera.transform.localRotation = this.ogLocalRot;

            player.enabled = true;
            this.ghostCamMode = false;
        }
    }

    void Update() {
        if (!Keyboard.current.equalsKey.wasPressedThisFrame) return;
        this.Toggle();
    }

    private void Log(string tag, string msg) {
        try {
            Console.Print(tag, msg);

        }
        catch {

        }
    }
}
