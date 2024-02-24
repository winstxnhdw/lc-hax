using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Hax;

internal class HaxCamera : MonoBehaviour {
    internal static HaxCamera? Instance { get; private set; }

    internal GameObject? CustomCameraObj { get; private set; }

    internal Camera? CustomCamera { get; private set; }

    internal void DestroyCustomCam() {
        if (Helper.LocalPlayer is not PlayerControllerB player) {
            Object.DestroyImmediate(CustomCameraObj);
            CustomCameraObj = null;
            CustomCamera = null;
            return;
        }

        if (Helper.StartOfRound is not StartOfRound { spectateCamera: Camera spectate }) return;
        if (!player.IsDead() && player.gameplayCamera is Camera camData) {
            camData.enabled = true;
        }
        else {
            spectate.enabled = true;
            if (CustomCameraObj != null) spectate.transform.position = CustomCameraObj.transform.position;
        }

        if (CustomCameraObj != null) {
            Object.DestroyImmediate(CustomCameraObj);
        }

        CustomCameraObj = null;
        CustomCamera = null;

    }

    void LateUpdate() {
        if (this.CustomCamera is null) return;
        if (Helper.StartOfRound is not StartOfRound round) return;
        if (round.spectateCamera is not Camera spectate) return;
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (player?.gameplayCamera is not Camera playercam) return;
        // keep the cameras off if custom camera is enabled
        if (playercam.enabled) playercam.enabled = false;
        if (spectate.enabled) spectate.enabled = false;
    }

    internal Camera? GetCamera(bool Spawn = true) {
        if (this.CustomCamera is not null) return this.CustomCamera;
        if(!Spawn) return this.CustomCamera;
        if (Helper.LocalPlayer is not PlayerControllerB player) return null;
        if (Helper.LocalPlayer?.gameplayCamera is not Camera playercam) return null;
        if (Helper.StartOfRound is not StartOfRound round) return null;
        if (round.spectateCamera is not Camera spectate) return null;

        Camera camData = player.IsDead() ? spectate : playercam;
        this.CustomCameraObj ??= new GameObject("lc-hax Camera");
        Camera newCam = this.CustomCameraObj.AddComponent<Camera>();
        newCam.transform.position = camData.transform.position;
        newCam.transform.rotation = camData.transform.rotation;

        // Copy camera settings
        newCam.CopyFrom(camData);

        // Ensure the custom camera has the same culling mask as the original camera
        newCam.cullingMask = camData.cullingMask;

        // Copy other relevant properties
        newCam.clearFlags = camData.clearFlags;
        newCam.backgroundColor = camData.backgroundColor;
        newCam.nearClipPlane = camData.nearClipPlane;
        newCam.farClipPlane = camData.farClipPlane;
        newCam.fieldOfView = camData.fieldOfView;
        newCam.depth = camData.depth;
        newCam.renderingPath = camData.renderingPath;

        // this makes it work as the actual camera
        newCam.tag = camData.tag;
        // add a listener to the camera using the same settings as the original camera 
        AudioListener? listener = newCam.gameObject.AddComponent<AudioListener>();
        if (listener != null) listener.enabled = true;
        if (camData.TryGetComponent(out HDAdditionalCameraData dataToCopy)) {
            HDAdditionalCameraData? hdData = newCam.gameObject.AddComponent<HDAdditionalCameraData>();
            if (hdData != null && dataToCopy != null) {
                hdData.customRenderingSettings = true;
                hdData.renderingPathCustomFrameSettingsOverrideMask.mask =
                    dataToCopy.renderingPathCustomFrameSettingsOverrideMask.mask;
                hdData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.CustomPass,
                    dataToCopy.renderingPathCustomFrameSettings.IsEnabled(FrameSettingsField.CustomPass));
                hdData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.Volumetrics,
                    dataToCopy.renderingPathCustomFrameSettings.IsEnabled(FrameSettingsField.Volumetrics));
                hdData.renderingPathCustomFrameSettings.lodBiasMode =
                    dataToCopy.renderingPathCustomFrameSettings.lodBiasMode;
                hdData.renderingPathCustomFrameSettings.lodBias = dataToCopy.renderingPathCustomFrameSettings.lodBias;
                hdData.antialiasing = dataToCopy.antialiasing;
                hdData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.ShadowMaps,
                    dataToCopy.renderingPathCustomFrameSettings.IsEnabled(FrameSettingsField.ShadowMaps));
            }
        }

        newCam.enabled = true;
        return this.CustomCamera = newCam;
    }


    internal void Awake() => Instance = this;

    internal void OnEnable() {
        GameListener.OnGameStart += this.DestroyCustomCam;
        GameListener.OnGameEnd += this.DestroyCustomCam;
    }

    internal void OnDisable() {
        GameListener.OnGameStart -= this.DestroyCustomCam;
        GameListener.OnGameEnd -= this.DestroyCustomCam;
    }
}
