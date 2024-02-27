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

    internal GameObject? HaxCamContainer { get; private set; }

    internal GameObject? HaxCamAudioContainer { get; private set; }

    internal Camera? CustomCamera { get; private set; }

    internal AudioListener? HaxCamAudioListener { get; private set; }

    internal void SetActive(bool active) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (this.HaxCamContainer is null) return;
        if (this.SpectateCamera is not Camera spectate) return;
        if (this.GameplayCamera is not Camera gameplayCamera) return;

        if (this.GetCamera() is not Camera cam) return;
        this.HaxCamContainer.SetActive(active);
        if (!this.HaxCamContainer.activeSelf) {
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
            this.HaxCamContainer.transform.SetParent(null, true);
            this.UpdateCameraTrasform(player.IsDead() ? spectate.transform : player.playerEye);
            spectate.gameObject.SetActive(false);
            gameplayCamera.gameObject.SetActive(false);
        }
    }


    internal void DisableCamera() {
        if (PhantomMod.Instance is PhantomMod phantom) phantom.DisablePhantom();
        this.HaxCamContainer?.SetActive(false);
    }


    internal Camera? GetCamera() {
        if (this.CustomCamera != null) return this.CustomCamera;

        this.HaxCamContainer ??= new GameObject("lc-hax Camera Parent");
        Camera newCam = this.HaxCamContainer.AddComponent<Camera>();

        this.HaxCamAudioContainer ??= new GameObject("lc-hax Audio Listener");
        this.HaxCamAudioListener = this.HaxCamAudioContainer.GetComponent<AudioListener>();
        this.HaxCamAudioListener ??= this.HaxCamAudioContainer.AddComponent<AudioListener>();
        this.HaxCamAudioListener.transform.SetParent(this.HaxCamContainer.transform, false);
        this.HaxCamAudioListener.transform.localScale = new Vector3(0.8196f, 0.8196f, 0.8196f);
        // zero both local pos and rot
        this.HaxCamAudioListener.transform.localPosition = Vector3.zero;
        this.HaxCamAudioListener.transform.localRotation = Quaternion.identity;

        // Set HaxCamAudioContainer as a child of HaxCamContainer
        this.HaxCamAudioContainer.transform.SetParent(this.HaxCamContainer.transform, false);

        // add AudioReverbFilter
        AudioReverbFilter? reverb = this.HaxCamAudioContainer.GetComponent<AudioReverbFilter>();
        reverb ??= this.HaxCamAudioContainer.gameObject.AddComponent<AudioReverbFilter>();
        reverb.enabled = true;

        AudioLowPassFilter? lowpass = this.HaxCamAudioContainer.GetComponent<AudioLowPassFilter>();
        lowpass ??= this.HaxCamAudioContainer.gameObject.AddComponent<AudioLowPassFilter>();
        lowpass.enabled = false;

        AudioChorusFilter? chorus = this.HaxCamAudioContainer.GetComponent<AudioChorusFilter>();
        chorus ??= this.HaxCamAudioContainer.gameObject.AddComponent<AudioChorusFilter>();
        chorus.enabled = false;


        if (this.GameplayListener is AudioListener gameplayListener) {
            if (gameplayListener.TryGetComponent(out AudioReverbFilter source)) {
                this.CopyReverbFilterSettings(source, reverb);
            }

            if (gameplayListener.TryGetComponent(out AudioLowPassFilter lowPassSource)) {
                this.CopyLowPassFilterSettings(lowPassSource, lowpass);
            }

            if (gameplayListener.TryGetComponent(out AudioChorusFilter chorusSource)) {
                this.CopyChorusFilterSettings(chorusSource, chorus);
            }
        }

        this.KeyboardMovement = newCam.GetComponent<KeyboardMovement>();
        this.KeyboardMovement ??= newCam.gameObject.AddComponent<KeyboardMovement>();


        this.MousePan = newCam.GetComponent<MousePan>();
        this.MousePan ??= newCam.gameObject.AddComponent<MousePan>();


        this.MousePan.enabled = true;
        this.KeyboardMovement.enabled = true;
        this.HaxCamAudioListener.enabled = true;
        this.HaxCamContainer.SetActive(false);
        this.CustomCamera = newCam;
        DontDestroyOnLoad(this.HaxCamContainer);
        return newCam;
    }


    internal void CopyLowPassFilterSettings(AudioLowPassFilter source, AudioLowPassFilter target) {
        target.cutoffFrequency = source.cutoffFrequency;
        target.lowpassResonanceQ = source.lowpassResonanceQ;
    }

    internal void CopyChorusFilterSettings(AudioChorusFilter source, AudioChorusFilter target) {
        target.dryMix = source.dryMix;
        target.wetMix1 = source.wetMix1;
        target.wetMix2 = source.wetMix2;
        target.wetMix3 = source.wetMix3;
        target.delay = source.delay;
        target.rate = source.rate;
        target.depth = source.depth;
    }

    internal void CopyReverbFilterSettings(AudioReverbFilter source, AudioReverbFilter target) {
        target.reverbPreset = source.reverbPreset;
        target.dryLevel = source.dryLevel;
        target.room = source.room;
        target.roomHF = source.roomHF;
        target.roomLF = source.roomLF;
        target.decayTime = source.decayTime;
        target.decayHFRatio = source.decayHFRatio;
        target.reflectionsLevel = source.reflectionsLevel;
        target.reflectionsDelay = source.reflectionsDelay;
        target.reverbLevel = source.reverbLevel;
        target.reverbDelay = source.reverbDelay;
        target.hfReference = source.hfReference;
        target.lfReference = source.lfReference;
        target.diffusion = source.diffusion;
        target.density = source.density;
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
        if (this.HaxCamContainer is not GameObject Cam) return;
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
