using System;
using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using Object = UnityEngine.Object;

sealed class PhantomCamera : IDisposable {
    internal Camera Camera { get; }
    internal bool IsActive => this.Root.activeSelf && this.Camera.enabled;

    GameObject Root { get; }
    KeyboardMovement KeyboardMovement { get; }
    MousePan MousePan { get; }
    AudioListener AudioListener { get; }
    HDAdditionalCameraData CameraData { get; }
    Camera? SourceCamera { get; set; }
    AudioListener? SourceListener { get; set; }
    bool SourceListenerEnabled { get; set; }

    internal PhantomCamera() {
        this.Root = new GameObject("Hax Phantom Camera");
        this.Camera = this.Root.AddComponent<Camera>();
        this.Camera.enabled = false;
        this.AudioListener = this.Root.AddComponent<AudioListener>();
        this.AudioListener.enabled = false;
        this.CameraData = this.Root.AddComponent<HDAdditionalCameraData>();
        this.KeyboardMovement = this.Root.AddComponent<KeyboardMovement>();
        this.KeyboardMovement.enabled = false;
        this.MousePan = this.Root.AddComponent<MousePan>();
        this.MousePan.enabled = false;
        this.Root.SetActive(false);

        Object.DontDestroyOnLoad(this.Root);
    }

    internal void Activate(StartOfRound round, Camera source, PlayerControllerB player) {
        Camera camera = this.Camera;
        this.SourceCamera = source;
        this.SourceListener = round.audioListener.Unfake() ?? player.activeAudioListener.Unfake();
        this.SourceListenerEnabled = this.SourceListener is { enabled: true };
        camera.CopyFrom(source);

        if (source.TryGetComponent(out HDAdditionalCameraData sourceData)) {
            sourceData.CopyTo(this.CameraData);
            this.CameraData.volumeAnchorOverride = camera.transform;
        }

        this.Root.transform.SetPositionAndRotation(source.transform.position, source.transform.rotation);
        this.Root.SetActive(true);
        this.TakeControl(round);
    }

    internal void TakeControl(StartOfRound round) {
        this.TakeCameraControl(round);
        this.TakeAudioControl(round);
    }

    void TakeCameraControl(StartOfRound round) {
        if (round.activeCamera != this.Camera) {
            Camera? sourceCamera = round.activeCamera.Unfake();
            this.SourceCamera = sourceCamera ?? this.SourceCamera;
            sourceCamera?.enabled = false;
            round.SwitchCamera(this.Camera);
        }

        this.Camera.enabled = true;
    }

    void TakeAudioControl(StartOfRound round) {
        if (round.audioListener == this.AudioListener) {
            this.AudioListener.enabled = true;
            return;
        }

        if (round.audioListener.Unfake() is AudioListener sourceListener) {
            this.SourceListener = sourceListener;
            this.SourceListenerEnabled = sourceListener.enabled;
        }

        this.SourceListener.Unfake()?.enabled = false;
        this.AudioListener.enabled = true;
        round.audioListener = this.AudioListener;
    }

    internal void SetMovementEnabled(bool enabled) {
        this.KeyboardMovement.enabled = enabled;
        this.MousePan.enabled = enabled;
    }

    internal void SetPosition(Vector3 position) {
        this.Root.transform.position = position;
        this.KeyboardMovement.LastPosition = position;
    }

    internal void Deactivate(StartOfRound round) {
        this.Root.SetActive(false);
        this.SetMovementEnabled(false);
        this.Camera.enabled = false;
        this.AudioListener.enabled = false;
        this.Root.tag = "Untagged";

        if (round.activeCamera == this.Camera && this.SourceCamera != null) {
            round.SwitchCamera(this.SourceCamera);
        }

        if (round.audioListener == this.AudioListener && this.SourceListener != null) {
            round.audioListener = this.SourceListener;
        }

        if (round.audioListener == this.SourceListener) {
            this.SourceListener.Unfake()?.enabled = this.SourceListenerEnabled;
        }
    }

    public void Dispose() => Object.Destroy(this.Root);
}
