using GameNetcodeStuff;
using Hax;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

internal class HaxCamera : MonoBehaviour
{
    internal KeyboardMovement? KeyboardMovement { get; private set; }
    internal MousePan? MousePan { get; private set; }


    internal static HaxCamera? Instance { get; private set; }

    internal GameObject? HaxCamContainer { get; private set; }

    internal GameObject? HaxCamAudioContainer { get; private set; }

    internal Camera? CustomCamera { get; private set; }

    internal AudioListener? HaxCamAudioListener { get; private set; }


    internal void OnEnable()
    {
        GameListener.OnGameStart += DisableCamera;
        GameListener.OnGameEnd += DisableCamera;
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (player.activeAudioListener is not AudioListener playerlistener) return;
        if (HaxCamContainer is null) return;
        if (HaxCamAudioListener is not AudioListener haxListener) return;
        if (GetCamera() is not Camera cam) return;
        HaxCamContainer.SetActive(true);
        CopyFromCamera(startOfRound.activeCamera.transform, ref cam, ref startOfRound.activeCamera);
        startOfRound.activeCamera.enabled = false;
        playerlistener.enabled = false;
        startOfRound.audioListener = haxListener;
    }

    internal void OnDisable()
    {
        GameListener.OnGameStart -= DisableCamera;
        GameListener.OnGameEnd -= DisableCamera;
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (player.activeAudioListener is not AudioListener playerlistener) return;
        if (HaxCamContainer is null) return;
        if (HaxCamAudioListener is not AudioListener haxListener) return;
        if (GetCamera() is not Camera cam) return;
        HaxCamContainer.SetActive(false);
        playerlistener.enabled = true;
        startOfRound.audioListener = playerlistener;
        startOfRound.activeCamera.enabled = true;
    }


    private void LateUpdate()
    {
        if (HaxCamContainer is null) return;
        if (KeyboardMovement is null) return;
        if (HaxCamContainer.activeSelf) KeyboardMovement.IsPaused = Helper.LocalPlayer is { isTypingChat: true };
    }


    internal void DisableCamera()
    {
        if (PhantomMod.Instance is PhantomMod phantom) phantom.SetPhantom(false);
        HaxCamContainer?.SetActive(false);
    }


    internal Camera? GetCamera()
    {
        if (CustomCamera != null) return CustomCamera;

        HaxCamContainer ??= new GameObject("lc-hax Camera Parent");
        var newCam = HaxCamContainer.AddComponent<Camera>();

        HaxCamAudioContainer ??= new GameObject("lc-hax Audio Listener");
        HaxCamAudioListener = HaxCamAudioContainer.GetComponent<AudioListener>();
        HaxCamAudioListener ??= HaxCamAudioContainer.AddComponent<AudioListener>();
        HaxCamAudioListener.transform.SetParent(HaxCamContainer.transform, false);
        HaxCamAudioListener.transform.localScale = new Vector3(0.8196f, 0.8196f, 0.8196f);
        // zero both local pos and rot
        HaxCamAudioListener.transform.localPosition = Vector3.zero;
        HaxCamAudioListener.transform.localRotation = Quaternion.identity;

        // Set HaxCamAudioContainer as a child of HaxCamContainer
        HaxCamAudioContainer.transform.SetParent(HaxCamContainer.transform, false);

        KeyboardMovement = newCam.GetComponent<KeyboardMovement>();
        KeyboardMovement ??= newCam.gameObject.AddComponent<KeyboardMovement>();


        MousePan = newCam.GetComponent<MousePan>();
        MousePan ??= newCam.gameObject.AddComponent<MousePan>();


        MousePan.enabled = true;
        KeyboardMovement.enabled = true;
        HaxCamAudioListener.enabled = true;
        HaxCamContainer.SetActive(false);
        CustomCamera = newCam;
        DontDestroyOnLoad(HaxCamContainer);
        return newCam;
    }


    internal void CopyFromCamera(Transform container, ref Camera Cam, ref Camera camData)
    {
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
        var hdData = Cam.GetComponent<HDAdditionalCameraData>();
        hdData ??= Cam.gameObject.AddComponent<HDAdditionalCameraData>();

        if (camData.TryGetComponent(out HDAdditionalCameraData dataToCopy))
        {
            if (hdData != null && dataToCopy != null)
            {
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
        else
        {
            Destroy(hdData);
        }

        UpdateCameraTrasform(container);
    }

    private void UpdateCameraTrasform(Transform target)
    {
        if (HaxCamContainer is not GameObject Cam) return;
        // Transform local position and rotation to world space
        var worldPosition = target.TransformPoint(target.localPosition);
        var worldRotation = target.rotation;
        // Copy transform properties
        Cam.transform.position = worldPosition;
        Cam.transform.rotation = worldRotation;
        Cam.gameObject.layer = target.gameObject.layer;

        if (KeyboardMovement != null) KeyboardMovement.LastPosition = worldPosition;
    }

    internal void Awake()
    {
        Instance = this;
        _ = GetCamera();
        enabled = false;
    }
}