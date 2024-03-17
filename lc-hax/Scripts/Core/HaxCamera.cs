using GameNetcodeStuff;
using Hax;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

internal class HaxCamera : MonoBehaviour {
    internal static HaxCamera? Instance { get; private set; }
    KeyboardMovement? KeyboardMovement { get; set; }
    MousePan? MousePan { get; set; }
    GameObject? HaxCameraAudioContainer { get; set; }
    AudioListener? HaxCameraAudioListener { get; set; }
    internal GameObject? HaxCameraContainer { get; private set; }
    internal Camera? CustomCamera { get; private set; }

    void OnEnable() {
        GameListener.OnGameStart += this.DisableCamera;
        GameListener.OnGameEnd += this.DisableCamera;

        Instance = this;
        _ = this.GetCamera();
    }

    void OnDisable() {
        GameListener.OnGameStart -= this.DisableCamera;
        GameListener.OnGameEnd -= this.DisableCamera;
    }

    void LateUpdate() {
        if (this.HaxCameraContainer is null) return;
        if (this.KeyboardMovement is null) return;
        if (!this.HaxCameraContainer.activeSelf) return;

        this.KeyboardMovement.IsPaused = Helper.LocalPlayer is { isTypingChat: true };
    }

    void DisableCamera() {
        if (PhantomMod.Instance is PhantomMod phantom) {
            phantom.DisablePhantom();
        }

        this.HaxCameraContainer?.SetActive(false);
    }

    internal void SetActive(bool active) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (player.activeAudioListener is not AudioListener playerlistener) return;
        if (this.HaxCameraContainer is null) return;
        if (this.HaxCameraAudioListener is not AudioListener haxListener) return;
        if (this.GetCamera() is not Camera camera) return;

        this.HaxCameraContainer.SetActive(active);

        if (!this.HaxCameraContainer.activeSelf) {
            playerlistener.enabled = true;
            startOfRound.audioListener = playerlistener;
            startOfRound.activeCamera.enabled = true;
        }

        else {
            this.CopyFromCamera(camera, startOfRound.activeCamera);
            startOfRound.activeCamera.enabled = false;
            playerlistener.enabled = false;
            startOfRound.audioListener = haxListener;
        }
    }

    Camera? GetCamera() {
        if (this.CustomCamera is not null) return this.CustomCamera;

        this.HaxCameraAudioContainer ??= new GameObject("HaxCamera Audio Container");
        this.HaxCameraContainer ??= new GameObject("HaxCamera Container");
        DontDestroyOnLoad(this.HaxCameraContainer);

        this.CustomCamera = this.HaxCameraContainer.AddComponent<Camera>();

        this.HaxCameraAudioListener =
            this.HaxCameraAudioContainer.GetComponent<AudioListener>() ??
            this.HaxCameraAudioContainer.AddComponent<AudioListener>();

        this.HaxCameraAudioContainer.transform.SetParent(this.HaxCameraContainer.transform, false);
        this.HaxCameraAudioListener.transform.SetParent(this.HaxCameraContainer.transform, false);
        this.HaxCameraAudioListener.transform.localScale = new Vector3(0.8196f, 0.8196f, 0.8196f);
        this.HaxCameraAudioListener.transform.localPosition = Vector3.zero;
        this.HaxCameraAudioListener.transform.localRotation = Quaternion.identity;

        this.KeyboardMovement =
            this.CustomCamera.GetComponent<KeyboardMovement>() ??
            this.CustomCamera.gameObject.AddComponent<KeyboardMovement>();

        this.MousePan =
            this.CustomCamera.GetComponent<MousePan>() ??
            this.CustomCamera.gameObject.AddComponent<MousePan>();

        this.MousePan.enabled = true;
        this.KeyboardMovement.enabled = true;
        this.HaxCameraAudioListener.enabled = true;
        this.HaxCameraContainer.SetActive(false);

        return this.CustomCamera;
    }

    void AddHDCameraCompatibility(Camera camera) {
        if (!camera.TryGetComponent(out HDAdditionalCameraData dataToCopy)) return;

        HDAdditionalCameraData hdData =
            camera.GetComponent<HDAdditionalCameraData>() ??
            camera.gameObject.AddComponent<HDAdditionalCameraData>();

        hdData.customRenderingSettings = true;
        hdData.renderingPathCustomFrameSettingsOverrideMask.mask = dataToCopy.renderingPathCustomFrameSettingsOverrideMask.mask;
        hdData.renderingPathCustomFrameSettings.lodBiasMode = dataToCopy.renderingPathCustomFrameSettings.lodBiasMode;
        hdData.renderingPathCustomFrameSettings.lodBias = dataToCopy.renderingPathCustomFrameSettings.lodBias;
        hdData.antialiasing = dataToCopy.antialiasing;

        hdData.renderingPathCustomFrameSettings.SetEnabled(
            FrameSettingsField.CustomPass,
            dataToCopy.renderingPathCustomFrameSettings.IsEnabled(FrameSettingsField.CustomPass)
        );

        hdData.renderingPathCustomFrameSettings.SetEnabled(
            FrameSettingsField.Volumetrics,
            dataToCopy.renderingPathCustomFrameSettings.IsEnabled(FrameSettingsField.Volumetrics)
        );

        hdData.renderingPathCustomFrameSettings.SetEnabled(
            FrameSettingsField.ShadowMaps,
            dataToCopy.renderingPathCustomFrameSettings.IsEnabled(FrameSettingsField.ShadowMaps)
        );
    }

    void UpdateCameraTransform(Transform target) {
        if (this.HaxCameraContainer is not GameObject cameraContainer) return;
        if (this.KeyboardMovement != null) {
            this.KeyboardMovement.LastPosition = target.position;
        }

        cameraContainer.transform.position = target.position;
        cameraContainer.transform.rotation = target.rotation;
    }

    void CopyFromCamera(Camera camera, Camera originalCamera) {
        camera.CopyFrom(originalCamera);
        camera.transform.localRotation = originalCamera.transform.localRotation;
        camera.backgroundColor = originalCamera.backgroundColor;
        camera.renderingPath = originalCamera.renderingPath;
        camera.nearClipPlane = originalCamera.nearClipPlane;
        camera.farClipPlane = originalCamera.farClipPlane;
        camera.depth = originalCamera.depth;
        camera.tag = originalCamera.tag;

        this.AddHDCameraCompatibility(camera);
        this.UpdateCameraTransform(originalCamera.transform);
    }
}
