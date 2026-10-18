using GameNetcodeStuff;
using UnityEngine;

sealed class AutoPlayInput(PlayerControllerB player) {
    static AutoPlayInput? Current { get; set; }

    PlayerControllerB Player { get; } = player;
    Vector3 WorldDirection { get; set; }
    bool Sprint { get; set; }
    bool IsPaused => this.Player.IsInputPaused() || this.Player.inSpecialMenu;

    internal static AutoPlayInput? GetInput(PlayerControllerB player) =>
        AutoPlayInput.Current is { } input &&
        input.Player == player && player is { isPlayerDead: false, isPlayerControlled: true }
            ? input
            : null;

    internal void Activate() => AutoPlayInput.Current = this;

    internal void Move(Vector3 worldDirection, bool sprint) {
        this.WorldDirection = worldDirection;
        this.Sprint = sprint;
    }

    internal void Stop() {
        this.WorldDirection = Vector3.zero;
        this.Sprint = false;
    }

    internal void Release() {
        if (AutoPlayInput.Current == this) {
            AutoPlayInput.Current = null;
        }

        if (this.Player.Unfake() is null) {
            return;
        }

        this.Stop();
        this.Player.inputHoldingSprint = false;
        this.Player.inputLetGoOfSprint = true;
        this.Player.smoothLookEnabledLastFrame = false;
    }

    internal Vector2 ReadMove() {
        if (this.IsPaused) return Vector2.zero;
        if (this.Player.isClimbingLadder) return Vector2.zero;

        Vector3 localDirection = this.Player.transform.InverseTransformDirection(this.WorldDirection);
        float horizontal = IngamePlayerSettings.Instance.flipCamera ? -localDirection.x : localDirection.x;
        return new Vector2(horizontal, localDirection.z);
    }

    internal float ReadSprint() {
        this.Player.inputHoldingSprint = false;
        this.Player.inputLetGoOfSprint = false;
        return !this.IsPaused && this.Sprint && this.WorldDirection.sqrMagnitude > 0.0f ? 1.0f : 0.0f;
    }

    internal void LookAt(Vector3 worldPosition) {
        if (this.Player.disableLookInput) return;

        Vector3 direction = worldPosition - this.Player.gameplayCamera.transform.position;

        if (direction.sqrMagnitude < 0.0001f) {
            return;
        }

        Vector3 bodyRotation = this.Player.thisPlayerBody.eulerAngles;
        bodyRotation.y = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        Vector3 cameraRotation = this.Player.gameplayCamera.transform.localEulerAngles;
        cameraRotation.x = this.Player.cameraUp;

        this.Player.thisPlayerBody.eulerAngles = bodyRotation;
        this.Player.cameraUp = Mathf.Clamp(-Mathf.Asin(direction.normalized.y) * Mathf.Rad2Deg, -80.0f, 80.0f);
        this.Player.gameplayCamera.transform.localEulerAngles = cameraRotation;
    }
}
