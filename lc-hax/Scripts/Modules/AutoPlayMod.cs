using UnityEngine;

[DefaultExecutionOrder(-55)]
[Mod]
sealed class AutoPlayMod : MonoBehaviour {
    bool IsActive { get; set; }
    AutoPlaySession? Session { get; set; }

    void OnEnable() {
        InputListener.OnF6Press += this.Toggle;
        GameListener.OnGameEnd += this.Stop;
    }

    void OnDisable() {
        InputListener.OnF6Press -= this.Toggle;
        GameListener.OnGameEnd -= this.Stop;
        this.Stop();
    }

    void Stop() {
        this.IsActive = false;
        this.StopSession();
    }

    void Toggle() {
        if (Helper.LocalPlayer is not { } player) return;
        if (player.IsInputPaused()) return;
        if (player.inSpecialMenu) return;

        this.IsActive = !this.IsActive;

        if (!this.IsActive) {
            this.StopSession();
        }

        Helper.SendNotification(title: "AutoPlay", body: this.IsActive ? "Enabled" : "Disabled");
    }

    void StopSession() {
        if (this.Session is not { } session) return;

        this.Session = null;
        session.Dispose();
    }

    void Update() {
        if (!this.IsActive) return;

        if (Helper.LocalPlayer is not { isPlayerDead: false, isPlayerControlled: true } player ||
            Helper.StartOfRound is not { shipHasLanded: true, shipIsLeaving: false, inShipPhase: false } ||
            player.GetComponent<KeyboardMovement>() != null
        ) {
            this.StopSession();
            return;
        }

        if (this.Session is AutoPlaySession previous && previous.Player != player) {
            this.StopSession();
        }

        if (this.Session is null) {
            if (player.inSpecialInteractAnimation) return;
            if (player.isClimbingLadder) return;

            player.StopHoldInteractionOnTrigger();
            this.Session = new AutoPlaySession(player);
            this.Session.Input.Activate();
        }

        this.Session.Tick();
    }
}
