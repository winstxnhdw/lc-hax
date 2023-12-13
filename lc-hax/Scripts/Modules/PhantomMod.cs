using UnityEngine;
using GameNetcodeStuff;
using System;

namespace Hax;

public class PhantomMod : MonoBehaviour {
    bool toggleOn = false;
    bool ghostCamMode = false;

    QuickKeyboardMoveAround? kInstance = null;
    QuickMouseCameraLookAround? mInstance = null;

    Vector3 ogLocalPos;
    Quaternion ogLocalRot;
    Transform? ogParent;

    int currentPlayerToTeleportToIndex = 0;

    Action? goNextPlayerFunc = null;
    Action? goPrevPlayerFunc = null;

    void OnEnable() {
        this.goNextPlayerFunc = () => { this.GoToPlayer(1); };
        this.goPrevPlayerFunc = () => { this.GoToPlayer(-1); };

        InputListener.onEqualsPress += this.TogglePhantom;
        InputListener.onRightArrowKeyPress += this.goNextPlayerFunc;
        InputListener.onLeftArrowKeyPress += this.goPrevPlayerFunc;
    }

    void OnDisable() {
        InputListener.onEqualsPress -= this.TogglePhantom;
        InputListener.onRightArrowKeyPress -= this.goNextPlayerFunc;
        InputListener.onLeftArrowKeyPress -= this.goPrevPlayerFunc;
    }

    void GoToPlayer(int indexChange) {
        if (!this.ghostCamMode) return;
        if (!Helper.Extant(Helper.CurrentCamera, out Camera camera)) return;

        int currentPlayerCount = Helper.Players?.Length ?? 0;

        if (currentPlayerCount <= 0) {
            this.currentPlayerToTeleportToIndex = -1;
            return;
        }

        int currentIndex = this.currentPlayerToTeleportToIndex;
        currentIndex += indexChange;

        currentIndex =
        //if more than playercount, loop back to 0
        currentIndex >= currentPlayerCount
        ?
        0
        :
        //if less than 0, loop forward
            currentIndex < 0
            ?
            currentPlayerCount - 1
            :
            currentIndex;

        PlayerControllerB? targetPlayer = Helper.Players?[currentIndex];
        if (targetPlayer == null) {
            this.Log($"Player is somehow null");
            return;
        }

        this.Log($"Teleporting to {targetPlayer.playerUsername}, index={currentIndex}");
        camera.transform.position = targetPlayer.playerEye.position;

        this.currentPlayerToTeleportToIndex = currentIndex;

    }

    void TogglePhantom() {
        if (!Helper.Extant(Helper.CurrentCamera, out Camera camera)) return;
        if (!camera.enabled) return;
        if (!Helper.Extant(Helper.LocalPlayer, out PlayerControllerB player)) return;

        if (this.ogParent == null) {
            this.ogParent = camera.transform.parent;
            return;
        }

        this.toggleOn = !this.toggleOn;

        this.Log($"{this.toggleOn} cam={camera}");

        if (this.toggleOn) {
            if (this.ghostCamMode) return;

            this.Log($"Enable cam");
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

    private void Log(string msg) {
        try {
            Console.Print("GHOST", msg);

        }
        catch {

        }
    }
}
