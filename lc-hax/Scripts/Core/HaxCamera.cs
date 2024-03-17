using GameNetcodeStuff;
using Hax;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

internal class HaxCamera : MonoBehaviour {

    internal KeyboardMovement? KeyboardMovement { get; private set; }
    internal MousePan? MousePan { get; private set; }


    internal static HaxCamera? Instance { get; private set; }

    internal GameObject? HaxCameraContainer { get; private set; }

    internal GameObject? HaxCamAudioContainer { get; private set; }

    internal Camera? CustomCamera { get; private set; }

    internal AudioListener? HaxCamAudioListener { get; private set; }

    internal void SetActive(bool active) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (player.activeAudioListener is not AudioListener playerlistener) return;
        if (this.HaxCameraContainer is null) return;
        if (this.HaxCamAudioListener is not AudioListener haxListener) return;
        if (this.GetCamera() is not Camera cam) return;
        this.HaxCameraContainer.SetActive(active);
        if (!this.HaxCameraContainer.activeSelf) {
            playerlistener.enabled = true;
            startOfRound.audioListener = playerlistener;
            startOfRound.activeCamera.enabled = true;
        }
        else {
            this.CopyFromCamera(startOfRound.activeCamera.transform, ref cam, ref startOfRound.activeCamera);
            startOfRound.activeCamera.enabled = false;
            playerlistener.enabled = false;
            startOfRound.audioListener = haxListener;
        }
    }

    void LateUpdate() {
        if (this.HaxCameraContainer is null) return;
        if (this.KeyboardMovement is null) return;
        if (this.HaxCameraContainer.activeSelf) {
            this.KeyboardMovement.IsPaused = Helper.LocalPlayer is { isTypingChat: true };
        }
    }


    internal void DisableCamera() {
        if (PhantomMod.Instance is PhantomMod phantom) phantom.DisablePhantom();
        this.HaxCameraContainer?.SetActive(false);
    }


    internal Camera? GetCamera() {
        if (this.CustomCamera != null) return this.CustomCamera;

        this.HaxCameraContainer ??= new GameObject("lc-hax Camera Parent");
        Camera newCam = this.HaxCameraContainer.AddComponent<Camera>();

        this.HaxCamAudioContainer ??= new GameObject("lc-hax Audio Listener");
        this.HaxCamAudioListener = this.HaxCamAudioContainer.GetComponent<AudioListener>();
        this.HaxCamAudioListener ??= this.HaxCamAudioContainer.AddComponent<AudioListener>();
        this.HaxCamAudioListener.transform.SetParent(this.HaxCameraContainer.transform, false);
        this.HaxCamAudioListener.transform.localScale = new Vector3(0.8196f, 0.8196f, 0.8196f);
        // zero both local pos and rot
        this.HaxCamAudioListener.transform.localPosition = Vector3.zero;
        this.HaxCamAudioListener.transform.localRotation = Quaternion.identity;

        // Set HaxCamAudioContainer as a child of HaxCamContainer
        this.HaxCamAudioContainer.transform.SetParent(this.HaxCameraContainer.transform, false);

        this.KeyboardMovement = newCam.GetComponent<KeyboardMovement>();
        this.KeyboardMovement ??= newCam.gameObject.AddComponent<KeyboardMovement>();


        this.MousePan = newCam.GetComponent<MousePan>();
        this.MousePan ??= newCam.gameObject.AddComponent<MousePan>();


        this.MousePan.enabled = true;
        this.KeyboardMovement.enabled = true;
        this.HaxCamAudioListener.enabled = true;
        this.HaxCameraContainer.SetActive(false);
        this.CustomCamera = newCam;
        DontDestroyOnLoad(this.HaxCameraContainer);
        return newCam;
    }


    internal void CopyFromCamera(Transform container, ref Camera Cam, ref Camera camData) {
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
        if (this.HaxCameraContainer is not GameObject camera) return;
        // Transform local position and rotation to world space
        Vector3 worldPosition = target.TransformPoint(target.localPosition);
        Quaternion worldRotation = target.rotation;
        // Copy transform properties
        camera.transform.position = worldPosition;
        camera.transform.rotation = worldRotation;
        camera.gameObject.layer = target.gameObject.layer;

        if (this.KeyboardMovement != null) {
            this.KeyboardMovement.LastPosition = worldPosition;
        }
    }

    internal void Awake() {
        Instance = this;
        _ = this.GetCamera();
    }

    void OnEnable() {
        GameListener.OnGameStart += this.DisableCamera;
        GameListener.OnGameEnd += this.DisableCamera;
    }

    void OnDisable() {
        GameListener.OnGameStart -= this.DisableCamera;
        GameListener.OnGameEnd -= this.DisableCamera;
    }
}
