using DunGen.Tags;
using GameNetcodeStuff;
using Hax;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

internal class HaxCamera : MonoBehaviour {

    internal KeyboardMovement? KeyboardMovement { get; private set; }
    internal MousePan? MousePan { get; private set; }
    internal Camera? GameplayCamera => Helper.LocalPlayer?.gameplayCamera;

    internal Camera? SpectateCamera => Helper.StartOfRound?.spectateCamera;

    internal AudioListener? GameplayListener => Helper.LocalPlayer?.activeAudioListener;
    internal AudioListener? SpectatorListener => Helper.StartOfRound?.audioListener;



    internal static HaxCamera? Instance { get; private set; }

    internal GameObject? CustomCameraObj { get; private set; }

    internal Camera? CustomCamera { get; private set; }

    internal AudioListener? CameraListener { get; private set; }

    internal void SetActive(bool active) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (this.SpectateCamera is not Camera spectate) return;
        if (this.GameplayCamera is not Camera gameplayCamera) return;
        if (this.CameraListener is not AudioListener CameraListener) return;

        if (this.GetCamera() is not Camera cam) return;
        cam.gameObject.SetActive(active);
        if (!cam.gameObject.activeSelf) {
            if (!player.IsDead()) {
                this.CopyFromCamera(player.playerEye, ref cam, ref gameplayCamera);
                gameplayCamera.gameObject.SetActive(true);
                spectate.gameObject.SetActive(false);
            }
            else {
                this.CopyFromCamera(spectate.transform, ref cam, ref spectate);
                spectate.gameObject.SetActive(true);
                gameplayCamera.gameObject.SetActive(false);
            }
        }
        else {
            cam.transform.SetParent(null, true);
            this.UpdateCameraTrasform(player.IsDead() ? spectate.transform : player.playerEye);
            spectate.gameObject.SetActive(false);
            gameplayCamera.gameObject.SetActive(false);
        }
    }

    void LateUpdate() {
        if (this.CustomCamera is null) return;
        if (this.SpectateCamera is not Camera spect) return;
        if (this.GameplayCamera is not Camera game) return;
        if (this.CustomCamera.gameObject.activeSelf) {
            if (game.gameObject.activeSelf) game.gameObject.SetActive(false);
            if (spect.gameObject.activeSelf) spect.gameObject.SetActive(false);
        }
    }

    internal void DisableCamera() {
        _ = this.GetCamera();
        this.SetActive(false);
        if (PhantomMod.Instance is PhantomMod phantom) phantom.DisablePhantom();
    }


    internal Camera? GetCamera() {
        if (this.CustomCamera is not null) return this.CustomCamera;

        this.CustomCameraObj ??= new GameObject("lc-hax Camera");
        Camera newCam = this.CustomCameraObj.AddComponent<Camera>();

        this.CameraListener = newCam.GetComponent<AudioListener>();
        this.CameraListener ??= newCam.gameObject.AddComponent<AudioListener>();


        this.KeyboardMovement = newCam.GetComponent<KeyboardMovement>();
        this.KeyboardMovement ??= newCam.gameObject.AddComponent<KeyboardMovement>();


        this.MousePan = newCam.GetComponent<MousePan>();
        this.MousePan ??= newCam.gameObject.AddComponent<MousePan>();


        this.MousePan.enabled = true;
        this.KeyboardMovement.enabled = true;
        this.CameraListener.enabled = true;
        newCam.enabled = true;
        this.CustomCameraObj.SetActive(false);
        UnityEngine.Object.DontDestroyOnLoad(this.CustomCameraObj);
        return this.CustomCamera = newCam;
    }


    internal void CopyFromCamera(Transform container, ref Camera Cam, ref Camera camData) {
        // zero all the transform properties
        Cam.transform.SetParent(container, false);
        Cam.transform.position = Vector3.zero;
        Cam.transform.rotation = Quaternion.identity;
        Cam.transform.localPosition = Vector3.zero;
        Cam.transform.localRotation = Quaternion.identity;
        Cam.transform.localScale = Vector3.one;

        // Copy camera settings
        Cam.CopyFrom(camData);

        // Ensure the custom camera has the same culling mask as the original camera
        Cam.cullingMask = camData.cullingMask;

        // Copy other relevant properties
        Cam.clearFlags = camData.clearFlags;
        Cam.backgroundColor = camData.backgroundColor;
        Cam.nearClipPlane = camData.nearClipPlane;
        Cam.farClipPlane = camData.farClipPlane;
        Cam.fieldOfView = camData.fieldOfView;
        Cam.depth = camData.depth;
        Cam.renderingPath = camData.renderingPath;

        // Make it work as the actual camera
        Cam.tag = camData.tag;

        // Get or add the HDAdditionalCameraData component
        HDAdditionalCameraData? hdData = Cam.GetComponent<HDAdditionalCameraData>();
        hdData ??= Cam.gameObject.AddComponent<HDAdditionalCameraData>();

        if (camData.TryGetComponent(out HDAdditionalCameraData dataToCopy)) {
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
        else {
            Destroy(hdData);
        }

        this.UpdateCameraTrasform(container);
    }

    void UpdateCameraTrasform(Transform target) {
        if (this.CustomCamera is not Camera Cam) return;
        // Transform local position and rotation to world space
        Vector3 worldPosition = target.TransformPoint(target.localPosition);
        Quaternion worldRotation = target.rotation;
        // Copy transform properties
        Cam.transform.position = worldPosition;
        Cam.transform.rotation = worldRotation;
        Cam.gameObject.layer = target.gameObject.layer;

        if (this.KeyboardMovement != null) {
            this.KeyboardMovement.LastPosition = worldPosition;
        }
    }

    internal void Awake() => Instance = this;

    internal void OnEnable() {
        GameListener.OnGameStart += this.DisableCamera;
        GameListener.OnGameEnd += this.DisableCamera;
    }

    internal void OnDisable() {
        GameListener.OnGameStart -= this.DisableCamera;
        GameListener.OnGameEnd -= this.DisableCamera;
    }
}
