using GameNetcodeStuff;
using Hax;
using UnityEngine;

sealed class PhantomMod : MonoBehaviour {

    internal static PhantomMod? Instance { get; private set; }
    bool IsShiftHeld { get; set; } = false;
    bool EnabledPossession { get; set; } = false;
    int CurrentSpectatorIndex { get; set; } = 0;

    void Awake() {
        PhantomMod.Instance = this;
    }

    void OnEnable() {
        InputListener.OnShiftButtonHold += this.HoldShift;
        InputListener.OnEqualsPress += this.TogglePhantom;
        InputListener.OnRightArrowKeyPress += this.LookAtNextPlayer;
        InputListener.OnLeftArrowKeyPress += this.LookAtPreviousPlayer;
    }

    void OnDisable() {
        InputListener.OnShiftButtonHold -= this.HoldShift;
        InputListener.OnEqualsPress -= this.TogglePhantom;
        InputListener.OnRightArrowKeyPress -= this.LookAtNextPlayer;
        InputListener.OnLeftArrowKeyPress -= this.LookAtPreviousPlayer;
    }

    void Update() {
        if (PossessionMod.Instance is not PossessionMod possessionMod) return;
        if (ScrapPossessionMod.Instance is not ScrapPossessionMod itemPossessionMod) return; // Assuming ScrapPossessionMod is your ItemPossessionMod

        if (Helper.CurrentCamera is not Camera { enabled: true } camera) return;
        if (!camera.gameObject.TryGetComponent(out KeyboardMovement keyboard)) return;
        if (!camera.gameObject.TryGetComponent(out MousePan mouse)) return;

        bool isPossessed = possessionMod.IsPossessed || itemPossessionMod.IsPossessed; // Combined possession check

        if (!Setting.EnablePhantom) {
            if (!this.EnabledPossession) return;

            if (possessionMod.IsPossessed) {
                possessionMod.Unpossess();
                possessionMod.enabled = false;
            }
            if (itemPossessionMod.IsPossessed) {
                itemPossessionMod.Unpossess();
                itemPossessionMod.enabled = false;
            }
            this.EnabledPossession = false;
        }
        else if (!isPossessed) { // If neither is currently possessed
            this.EnabledPossession = false;
            keyboard.enabled = true;
            mouse.enabled = true;
        }
        else if (!this.EnabledPossession) { // Possessing for the first frame
            this.EnabledPossession = true;
            if (possessionMod.IsPossessed) possessionMod.enabled = true;
            else if (itemPossessionMod.IsPossessed) itemPossessionMod.enabled = true;
            keyboard.enabled = false;
            mouse.enabled = false;
        }

        if (!isPossessed && Setting.EnablePhantom) {
            keyboard.IsPaused = Helper.LocalPlayer is { isTypingChat: true };
        }
    }


    void HoldShift(bool isHeld) => this.IsShiftHeld = isHeld;

    void LookAtNextPlayer() => this.LookAtPlayer(1);

    void LookAtPreviousPlayer() => this.LookAtPlayer(-1);

    void LookAtPlayer(int indexChange) {
        if (!Setting.EnablePhantom || Helper.CurrentCamera is not Camera camera) return;
        if (!camera.gameObject.TryGetComponent(out KeyboardMovement keyboard)) return;

        int playerCount = Helper.Players?.Length ?? 0;
        this.CurrentSpectatorIndex = (this.CurrentSpectatorIndex + indexChange) % playerCount;


        if (Helper.GetActivePlayer(this.CurrentSpectatorIndex) is not PlayerControllerB targetPlayer) {
            this.LookAtNextPlayer();
            return;
        }

        keyboard.LastPosition = targetPlayer.playerEye.position;
    }

    void PhantomEnabled(PlayerControllerB player, Camera camera) {
        if (!camera.TryGetComponent(out KeyboardMovement keyboard)) {
            keyboard = camera.gameObject.AddComponent<KeyboardMovement>();
        }

        if (!camera.TryGetComponent(out MousePan mouse)) {
            mouse = camera.gameObject.AddComponent<MousePan>();
        }

        keyboard.enabled = true;
        mouse.enabled = true;
    }

    void PhantomDisabled(PlayerControllerB player, Camera camera) {
        if (player.gameplayCamera is not Camera gameplayCamera) return;
        if (Helper.StartOfRound is not StartOfRound round) return;
        if(HaxCamera.Instance is not HaxCamera haxCamera) return;
        if(haxCamera.HaxCamContainer is not GameObject container) return;

        if (this.IsShiftHeld) {
            player.TeleportPlayer(container.transform.position);
        }
        if (PossessionMod.Instance is PossessionMod { IsPossessed: true } possession) {
            possession.Unpossess();
        }
    }

    void TogglePhantom() {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (HaxCamera.Instance is not HaxCamera haxCamera) return;
        if (haxCamera.GetCamera() is not Camera camera) return;
        Setting.EnablePhantom = !Setting.EnablePhantom;
        player.enabled = !player.IsDead() || !Setting.EnablePhantom;
        player.playerBodyAnimator.enabled = !Setting.EnablePhantom;
        player.thisController.enabled = !Setting.EnablePhantom;
        player.isFreeCamera = Setting.EnablePhantom;
        if (Setting.EnablePhantom) {
            this.PhantomEnabled(player, camera);
        }
        else {
            this.PhantomDisabled(player, camera);
        }

        haxCamera.SetActive(Setting.EnablePhantom);
    }

    internal void DisablePhantom() {
        Setting.EnablePhantom = false;
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (HaxCamera.Instance is not HaxCamera haxCamera) return;
        if (haxCamera.GetCamera() is not Camera camera) return;
        player.enabled = !player.IsDead();
        player.playerBodyAnimator.enabled = true;
        player.thisController.enabled = true;
        player.isFreeCamera = false;
        this.PhantomDisabled(player, camera);

    }
}
