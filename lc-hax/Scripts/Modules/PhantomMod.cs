using GameNetcodeStuff;
using UnityEngine;

// Process input before camera control, and prepare movement before CharacterMovement.
[DefaultExecutionOrder(50)]
sealed class PhantomMod : MonoBehaviour {
    PhantomCamera? PhantomCamera { get; set; }
    IPhantomExtension? Extension { get; set; }
    PlayerControllerB? Player { get; set; }

    bool PlayerEnabled { get; set; }
    bool AnimatorEnabled { get; set; }
    bool ControllerEnabled { get; set; }
    bool WasFreeCamera { get; set; }
    int CurrentSpectatorIndex { get; set; }

    void Awake() => this.PhantomCamera = new PhantomCamera();

    void OnEnable() {
        InputListener.OnEqualsPress += this.TogglePhantom;
        InputListener.OnRightArrowKeyPress += this.LookAtNextPlayer;
        InputListener.OnLeftArrowKeyPress += this.LookAtPreviousPlayer;
        GameListener.OnGameEnd += this.DisablePhantom;
    }

    void OnDisable() {
        InputListener.OnEqualsPress -= this.TogglePhantom;
        InputListener.OnRightArrowKeyPress -= this.LookAtNextPlayer;
        InputListener.OnLeftArrowKeyPress -= this.LookAtPreviousPlayer;
        GameListener.OnGameEnd -= this.DisablePhantom;
        this.DisablePhantom();
    }

    void OnDestroy() => this.PhantomCamera?.Dispose();

    void Update() {
        if (!Setting.EnablePhantom) return;

        this.Extension?.Update();
        this.UpdateCameraMovement();
    }

    void LateUpdate() {
        if (!Setting.EnablePhantom) return;
        if (Helper.StartOfRound is StartOfRound round) {
            this.PhantomCamera?.TakeControl(round);
        }

        this.Extension?.LateUpdate();
        this.UpdateCameraMovement();
    }

    void UpdateCameraMovement() => this.PhantomCamera?.SetMovementEnabled(
        this.Player.Unfake() is PlayerControllerB player && this.Extension is not { ControlsCamera: true } && !player.IsInputPaused()
    );

    void LookAtNextPlayer() => this.LookAtPlayer(1);

    void LookAtPreviousPlayer() => this.LookAtPlayer(-1);

    void LookAtPlayer(int indexChange) {
        if (!Setting.EnablePhantom) return;
        if (this.Player.Unfake() is not { } player) return;
        if (player.IsInputPaused()) return;
        if (this.Extension is { ControlsCamera: true }) return;
        if (this.PhantomCamera is null) return;
        if (Helper.ActivePlayers is not { Length: > 0 and var playerCount } players) return;

        this.CurrentSpectatorIndex = (this.CurrentSpectatorIndex + indexChange + playerCount) % playerCount;
        this.PhantomCamera.SetPosition(players[this.CurrentSpectatorIndex].playerEye.position);
    }

    void TogglePhantom(bool teleportOnExit) {
        if (Helper.LocalPlayer is not { } player) return;
        if (player.IsInputPaused()) return;
        if (!Setting.EnablePhantom) {
            this.EnablePhantom(player);
            return;
        }

        Vector3? returnPosition = teleportOnExit && !player.isPlayerDead ? this.PhantomCamera?.Camera?.transform.position : null;
        this.DisablePhantom();

        if (returnPosition is Vector3 position) {
            player.TeleportPlayer(position);
        }
    }

    void EnablePhantom(PlayerControllerB player) {
        if (Helper.StartOfRound is not { } round) return;
        if (round.activeCamera is not { } source) return;
        if (this.PhantomCamera is null) return;

        this.Player = player;
        this.PlayerEnabled = player.enabled;
        this.AnimatorEnabled = player.playerBodyAnimator.enabled;
        this.ControllerEnabled = player.thisController.enabled;
        this.WasFreeCamera = player.isFreeCamera;

        this.PhantomCamera.Activate(round, source, player);
        Setting.EnablePhantom = true;
        this.Extension = new Possession(this.PhantomCamera.Camera);

        player.enabled = true;
        player.playerBodyAnimator.enabled = false;
        player.thisController.enabled = false;
        player.isFreeCamera = true;
        this.UpdateCameraMovement();
    }

    internal void DisablePhantom() {
        if (!Setting.EnablePhantom) return;
        if (this.PhantomCamera is null) return;
        if (Helper.StartOfRound is not { } round) return;

        PlayerControllerB? player = this.Player.Unfake();
        Setting.EnablePhantom = false;
        this.Extension?.Dispose();
        this.PhantomCamera.Deactivate(round);
        player?.enabled = this.PlayerEnabled;
        player?.playerBodyAnimator.enabled = this.AnimatorEnabled;
        player?.thisController.enabled = this.ControllerEnabled;
        player?.isFreeCamera = this.WasFreeCamera;
    }
}
