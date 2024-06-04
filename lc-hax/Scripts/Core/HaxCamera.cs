#region

using System;
using System.Text;
using GameNetcodeStuff;
using Hax;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using static UnityEngine.EventSystems.EventTrigger;

#endregion

class HaxCamera : MonoBehaviour {

    internal KeyboardMovement? KeyboardMovement { get; private set; }
    internal MousePan? MousePan { get; private set; }

    internal static HaxCamera? Instance { get; private set; }

    internal GameObject? HaxCamContainer { get; private set; }

    internal GameObject? HaxCamAudioContainer { get; private set; }

    internal Camera? CustomCamera { get; private set; }

    internal AudioListener? HaxCamAudioListener { get; private set; }
    private bool wasInteracting = false;

    internal void Awake() {
        if (Instance != null) {
            Destroy(this);
            return;
        }

        Instance = this;
        _ = this.GetCamera();
        this.enabled = false;
    }


    

    internal void ClearCursor() {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        player.cursorIcon.enabled = false;
        player.cursorTip.text = "";
        if (player.hoveringOverTrigger != null) player.previousHoveringOverTrigger = player.hoveringOverTrigger;
        player.hoveringOverTrigger = null;
    }

    internal void SetOnlyCursorText(string Text) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        player.cursorTip.text = Text;
        player.cursorIcon.enabled = false;
        player.cursorTip.text = this.FixCursorTipText(player.cursorTip.text);
    }

    internal void SetCursorFromInteractTrigger(InteractTrigger interactTrigger) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (interactTrigger == null) {
            this.ClearCursor();
            return;
        }

        if (interactTrigger.isPlayingSpecialAnimation) {
            this.ClearCursor();
            return;
        }

        if (interactTrigger.interactable) {
            player.cursorIcon.enabled = true;
            player.cursorIcon.sprite = interactTrigger.hoverIcon;
            player.cursorTip.text = interactTrigger.hoverTip;
        }
        else {
            player.cursorIcon.sprite = interactTrigger.disabledHoverIcon;
            player.cursorIcon.enabled = interactTrigger.disabledHoverIcon != null;
            player.cursorTip.text = interactTrigger.disabledHoverTip;
        }

        player.cursorIcon.enabled = player.cursorIcon.sprite != null;
        player.cursorTip.text = this.FixCursorTipText(player.cursorTip.text);

    }

    internal void SetCursorFromGrabbable(GrabbableObject grabbable) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (grabbable == null) {
            this.ClearCursor();
            return;
        }

        if (!player.HasFreeSlots()) {
            player.cursorIcon.enabled = false;
            player.cursorTip.text = "Inventory Full";
            return;
        }


        player.cursorIcon.enabled = true;
        player.cursorIcon.sprite = player.grabItemIcon;
        if (!string.IsNullOrEmpty(grabbable.customGrabTooltip))
            player.cursorTip.text = grabbable.customGrabTooltip;
        else
            player.cursorTip.text = "Grab : [E]";

        player.cursorIcon.enabled = player.cursorIcon.sprite != null;
        player.cursorTip.text = this.FixCursorTipText(player.cursorTip.text);
    }


    internal string FixCursorTipText(string text) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return text;
        if (startOfRound.localPlayerUsingController) {
            StringBuilder stringBuilder = new StringBuilder(text);
            stringBuilder.Replace("[E]", "[X]");
            stringBuilder.Replace("[LMB]", "[X]");
            stringBuilder.Replace("[RMB]", "[R-Trigger]");
            stringBuilder.Replace("[F]", "[R-Shoulder]");
            return stringBuilder.ToString();
        }

        return text.Replace("[LMB]", "[E]");
    }
    internal void OnEnable() {
        GameListener.OnGameStart += this.DisableCamera;
        GameListener.OnGameEnd += this.DisableCamera;
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (player.activeAudioListener is not AudioListener playerlistener) return;
        if (this.HaxCamContainer is null) return;
        if (this.HaxCamAudioListener is not AudioListener haxListener) return;
        if (this.GetCamera() is not Camera cam) return;
        this.HaxCamContainer.SetActive(true);
        this.CopyFromCamera(startOfRound.activeCamera.transform, ref cam, ref startOfRound.activeCamera);
        startOfRound.activeCamera.enabled = false;
        playerlistener.enabled = false;
        startOfRound.audioListener = haxListener;
    }

    internal void OnDisable() {
        GameListener.OnGameStart -= this.DisableCamera;
        GameListener.OnGameEnd -= this.DisableCamera;
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (player.activeAudioListener is not AudioListener playerlistener) return;
        if (this.HaxCamContainer is null) return;
        if (this.HaxCamAudioListener is not AudioListener haxListener) return;
        if (this.GetCamera() is not Camera cam) return;
        this.HaxCamContainer.SetActive(false);
        playerlistener.enabled = true;
        startOfRound.audioListener = playerlistener;
        startOfRound.activeCamera.enabled = true;
        this.ClearCursor();
    }

    void LateUpdate() {
        if (this.HaxCamContainer is null) return;
        if (this.KeyboardMovement is null) return;
        if (!this.HaxCamContainer.activeSelf) return;
        this.OnCameraInteract();
        this.KeyboardMovement.IsPaused = Helper.LocalPlayer is { isTypingChat: true };
    }

    void OnCameraInteract() {
        if (Helper.CurrentCamera is not Camera camera) return;
        if (Helper.LocalPlayer is not PlayerControllerB localplayer) return;

        bool isInteracting =
            IngamePlayerSettings.Instance.playerInput.actions.FindAction("Interact", false).IsPressed();
        bool isPossessingEnemy = PossessionMod.Instance?.PossessedEnemy != null;

        foreach (RaycastHit raycastHit in camera.transform.SphereCastForward()) {
            Collider? collider = raycastHit.collider;
            Transform rootTransform = collider.transform.root;

            if (collider.TryGetComponent(out TerminalAccessibleObject terminalObject)) {
                if (terminalObject.isBigDoor) {
                    if (terminalObject.isDoorOpen())
                        this.SetOnlyCursorText($"Close {terminalObject.objectCode} Big Door [M3]");
                    else
                        this.SetOnlyCursorText($"Open {terminalObject.objectCode} Big Door [M3]");
                }
            }

            if (collider.TryGetComponent(out Turret turret)) {
                if (!Setting.EnableStunOnLeftClick)
                    this.SetOnlyCursorText($"Start Berserk Mode [M3]");
                else {
                    if (turret.isTurretActive())
                        this.SetOnlyCursorText("Set Turret Off [M3]");
                    else
                        this.SetOnlyCursorText("Set Turret On [M3]");
                }
            }

            if (rootTransform.name.ToLower().Contains("spikerooftraphazard")) {
                SpikeRoofTrap spike = rootTransform.GetComponentInChildren<SpikeRoofTrap>();
                if (spike != null) {
                    if (!Setting.EnableStunOnLeftClick)
                        this.SetOnlyCursorText($"Trigger Spike Slam [M3]");
                    else {
                        if (spike.isTrapActive())
                            this.SetOnlyCursorText("Set Spike Trap Off [M3]");
                        else
                            this.SetOnlyCursorText("Set Spike Trap On [M3]");
                    }
                }
            }

            if (collider.TryGetComponent(out Landmine landmine)) {
                if (!Setting.EnableStunOnLeftClick)
                    this.SetOnlyCursorText($"Explode Landmine [M3]");
                else {
                    if (landmine.isLandmineActive())
                        this.SetOnlyCursorText("Set Landmine Off [M3]");
                    else
                        this.SetOnlyCursorText("Set Landmine On [M3]");
                }

                break;
            }

            if (collider.TryGetComponent(out JetpackItem jetpack)) {
                this.SetOnlyCursorText($"Explode Jetpack [M3]");
                break;
            }

            if (collider.TryGetComponent(out DoorLock doorLock)) {
                // extract interactTrigger and use it
                InteractTrigger? trigger = doorLock.Reflect().GetInternalField<InteractTrigger>("doorTrigger");
                if (trigger != null) {
                    this.SetCursorFromInteractTrigger(trigger);
                    if (isPossessingEnemy) break;
                    if (isInteracting && !this.wasInteracting) {
                        if (doorLock.isLocked)
                            doorLock.UnlockDoorServerRpc();
                        else
                            trigger.Interact(Helper.LocalPlayer?.transform);

                        this.wasInteracting = true;
                    }
                }

                break;
            }

            if (collider.GetComponentInParent<EnemyAI>().Unfake() is EnemyAI enemy && Setting.EnablePhantom &&
                PossessionMod.Instance?.PossessedEnemy != enemy) {
                this.SetOnlyCursorText($"Possess {enemy.enemyType.enemyName} [M3]");
                break;
            }

            if (collider.TryGetComponent(out PlayerControllerB player)) {
                this.SetOnlyCursorText(
                    $"Set {player.GetPlayerUsername()} as Enemy Target. [M3] \n Follow Player [F + M3]");
                break;
            }

            if (collider.TryGetComponent(out DepositItemsDesk depositItemDesk)) {
                this.SetOnlyCursorText("Jeb : Attack Players [M3]");
                break;
            }

            if (collider.TryGetComponent(out InteractTrigger interactTrigger)) {
                this.SetCursorFromInteractTrigger(interactTrigger);
                if (isInteracting && !this.wasInteracting) {
                    interactTrigger.Interact(Helper.LocalPlayer?.transform);
                    this.ClearCursor();
                    this.wasInteracting = true;
                }

                break;
            }

            if (collider.TryGetComponent(out GrabbableObject item)) {
                if (isPossessingEnemy) break;
                if(localplayer.IsDead()) return;
                this.SetCursorFromGrabbable(item);
                if (isInteracting && !this.wasInteracting) {
                    localplayer.GrabObject(item);
                    this.ClearCursor();
                    this.wasInteracting = true;
                }

                break;
            }
        }

        if (!isInteracting) {
            this.wasInteracting = false;
        }
    }




    internal void DisableCamera() {
        if (PhantomMod.Instance is PhantomMod phantom) phantom.SetPhantom(false);
        this.HaxCamContainer?.SetActive(false);
    }

    internal Camera? GetCamera() {
        if (this.CustomCamera != null) return this.CustomCamera;

        this.HaxCamContainer ??= new GameObject("lc-hax Camera Parent");
        Camera? newCam = this.HaxCamContainer.AddComponent<Camera>();

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
        else
            Destroy(hdData);

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

        if (this.KeyboardMovement != null) this.KeyboardMovement.LastPosition = worldPosition;
    }


}
