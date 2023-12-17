using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class PhantomMod : MonoBehaviour {
    bool EnablePhantom { get; set; } = false;
    int CurrentSpectatorIndex { get; set; } = 0;


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
            Console.Print("Player not found!");
            return;
        }

        camera.transform.position = targetPlayer.playerEye.position;
        Console.Print($"Spectating {targetPlayer.playerUsername}");
    }

    void TogglePhantom() {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB player)) return;
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera) || !camera.enabled) return;
        if (!player.cameraContainerTransform.IsNotNull(out Transform cameraParent)) return;

        GameObject cameraGameObject = camera.gameObject;
        this.EnablePhantom = !this.EnablePhantom;
        player.enabled = !this.EnablePhantom;

        if (this.EnablePhantom) {
            if (!cameraGameObject.GetComponent<KeyboardMovement>().IsNotNull(out KeyboardMovement keyboard)) {
                keyboard = cameraGameObject.AddComponent<KeyboardMovement>();
            }

            if (!cameraGameObject.GetComponent<MousePan>().IsNotNull(out MousePan mouse)) {
                mouse = cameraGameObject.AddComponent<MousePan>();
            }

            keyboard.enabled = true;
            mouse.enabled = true;
            camera.transform.SetParent(null, true);
        }

        else {
            camera.transform.SetParent(cameraParent, false);
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localRotation = Quaternion.identity;

            if (cameraGameObject.GetComponent<KeyboardMovement>().IsNotNull(out KeyboardMovement keyboard)) {
                keyboard.enabled = false;
            }

            if (cameraGameObject.GetComponent<MousePan>().IsNotNull(out MousePan mouse)) {
                mouse.enabled = false;
            }

            if (!player.gameplayCamera.IsNotNull(out Camera gameplayCam)) {
                return;
            }

            if (gameplayCam.GetComponent<KeyboardMovement>().IsNotNull(out KeyboardMovement gameplayKeyboard)) {
                gameplayKeyboard.enabled = false;
            }

            if (gameplayCam.GetComponent<MousePan>().IsNotNull(out MousePan gameplayMouse)) {
                gameplayMouse.enabled = false;
            }
        }
    }
}
