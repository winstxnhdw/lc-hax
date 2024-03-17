using GameNetcodeStuff;
using Hax;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

internal class HaxCamera : MonoBehaviour {
    internal static HaxCamera? Instance { get; private set; }
    internal KeyboardMovement? KeyboardMovement { get; private set; }
    internal MousePan? MousePan { get; private set; }
    internal GameObject? HaxCameraContainer { get; private set; }
    internal GameObject? HaxCamAudioContainer { get; private set; }
    internal Camera? CustomCamera { get; private set; }
    internal AudioListener? HaxCamAudioListener { get; private set; }

    void Awake() {
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

    internal void SetActive(bool active) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (player.activeAudioListener is not AudioListener playerlistener) return;
        if (this.HaxCameraContainer is null) return;
        if (this.HaxCamAudioListener is not AudioListener haxListener) return;
        if (this.GetCamera() is not Camera camera) return;

        this.HaxCameraContainer.SetActive(active);

        if (!this.HaxCameraContainer.activeSelf) {
            playerlistener.enabled = true;
            startOfRound.audioListener = playerlistener;
            startOfRound.activeCamera.enabled = true;
        }
        else {
            this.CopyFromCamera(startOfRound.activeCamera.transform, camera, startOfRound.activeCamera);
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
        if (PhantomMod.Instance is PhantomMod phantom) {
            phantom.DisablePhantom();
        }

        this.HaxCameraContainer?.SetActive(false);
    }

    internal Camera? GetCamera() {
        if (this.CustomCamera != null) return this.CustomCamera;

        this.HaxCameraContainer ??= new GameObject("lc-hax Camera Parent");
        Camera newCamera = this.HaxCameraContainer.AddComponent<Camera>();

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

        this.KeyboardMovement = newCamera.GetComponent<KeyboardMovement>();
        this.KeyboardMovement ??= newCamera.gameObject.AddComponent<KeyboardMovement>();

        this.MousePan = newCamera.GetComponent<MousePan>();
        this.MousePan ??= newCamera.gameObject.AddComponent<MousePan>();

        this.MousePan.enabled = true;
        this.KeyboardMovement.enabled = true;
        this.HaxCamAudioListener.enabled = true;
        this.HaxCameraContainer.SetActive(false);
        this.CustomCamera = newCamera;
        DontDestroyOnLoad(this.HaxCameraContainer);
        return newCamera;
    }

    internal void CopyFromCamera(Transform container, Camera camera, Camera originalCamera) {
        camera.CopyFrom(originalCamera);

        camera.backgroundColor = originalCamera.backgroundColor;
        camera.renderingPath = originalCamera.renderingPath;
        camera.nearClipPlane = originalCamera.nearClipPlane;
        camera.farClipPlane = originalCamera.farClipPlane;
        camera.depth = originalCamera.depth;
        camera.tag = originalCamera.tag;

        // Get or add the HDAdditionalCameraData component
        HDAdditionalCameraData? hdData = camera.GetComponent<HDAdditionalCameraData>();
        hdData ??= camera.gameObject.AddComponent<HDAdditionalCameraData>();

        if (originalCamera.TryGetComponent(out HDAdditionalCameraData dataToCopy)) {
            if (hdData != null && dataToCopy != null) {
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
}
