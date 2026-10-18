using UnityEngine;

[DefaultExecutionOrder(-100)]
sealed class KeyboardMovement : MonoBehaviour {
    float SprintMultiplier { get; set; } = 1;
    bool IsWHeld { get; set; }
    bool IsAHeld { get; set; }
    bool IsSHeld { get; set; }
    bool IsDHeld { get; set; }
    bool IsSpaceHeld { get; set; }
    bool IsCtrlHeld { get; set; }
    bool IsLeftShiftHeld { get; set; }
    bool IsRightShiftHeld { get; set; }
    bool IsShiftHeld => this.IsLeftShiftHeld || this.IsRightShiftHeld;

    unsafe Vector3 Direction {
        get {
            bool right = this.IsDHeld;
            bool left = this.IsAHeld;
            bool up = this.IsSpaceHeld;
            bool down = this.IsCtrlHeld;
            bool forward = this.IsWHeld;
            bool backward = this.IsSHeld;

            Vector3 direction = new(
                *(byte*)&right - *(byte*)&left,
                *(byte*)&up - *(byte*)&down,
                *(byte*)&forward - *(byte*)&backward
            );

            return direction.normalized;
        }
    }

    internal Vector3 LastPosition { get; set; }
    internal bool IsPaused { get; set; }
    internal bool IsMoving { get; private set; }
    internal bool IsSprinting { get; private set; }

    void OnEnable() {
        this.LastPosition = this.transform.position;
        this.ResetMovement();
        InputListener.OnWButtonHold += this.HoldW;
        InputListener.OnAButtonHold += this.HoldA;
        InputListener.OnSButtonHold += this.HoldS;
        InputListener.OnDButtonHold += this.HoldD;
        InputListener.OnSpaceButtonHold += this.HoldSpace;
        InputListener.OnCtrlButtonHold += this.HoldCtrl;
        InputListener.OnShiftButtonHold += this.HoldLeftShift;
        InputListener.OnRightShiftButtonHold += this.HoldRightShift;
    }

    void OnDisable() {
        InputListener.OnWButtonHold -= this.HoldW;
        InputListener.OnAButtonHold -= this.HoldA;
        InputListener.OnSButtonHold -= this.HoldS;
        InputListener.OnDButtonHold -= this.HoldD;
        InputListener.OnSpaceButtonHold -= this.HoldSpace;
        InputListener.OnCtrlButtonHold -= this.HoldCtrl;
        InputListener.OnShiftButtonHold -= this.HoldLeftShift;
        InputListener.OnRightShiftButtonHold -= this.HoldRightShift;
        this.IsWHeld = false;
        this.IsAHeld = false;
        this.IsSHeld = false;
        this.IsDHeld = false;
        this.IsSpaceHeld = false;
        this.IsCtrlHeld = false;
        this.IsLeftShiftHeld = false;
        this.IsRightShiftHeld = false;
        this.ResetMovement();
    }

    void HoldW(bool isHeld) => this.IsWHeld = isHeld;

    void HoldA(bool isHeld) => this.IsAHeld = isHeld;

    void HoldS(bool isHeld) => this.IsSHeld = isHeld;

    void HoldD(bool isHeld) => this.IsDHeld = isHeld;

    void HoldSpace(bool isHeld) => this.IsSpaceHeld = isHeld;

    void HoldCtrl(bool isHeld) => this.IsCtrlHeld = isHeld;

    void HoldLeftShift(bool isHeld) => this.IsLeftShiftHeld = isHeld;

    void HoldRightShift(bool isHeld) => this.IsRightShiftHeld = isHeld;

    void ResetMovement() {
        this.SprintMultiplier = 1.0f;
        this.IsMoving = false;
        this.IsSprinting = false;
    }

    void LateUpdate() {
        if (this.IsPaused) {
            this.ResetMovement();
            return;
        }

        if (Helper.LocalPlayer is { isTypingChat: true }) {
            this.ResetMovement();
            return;
        }

        if (Helper.LocalPlayer?.quickMenuManager is { isMenuOpen: true }) {
            this.ResetMovement();
            return;
        }

        Vector3 direction = this.Direction;
        this.IsMoving = direction.sqrMagnitude > 0.0f;
        this.IsSprinting = this.IsMoving && this.IsShiftHeld;
        this.UpdateSprintMultiplier(this.IsShiftHeld);
        this.Move(direction);
    }

    void UpdateSprintMultiplier(bool isSprintHeld) =>
        this.SprintMultiplier = isSprintHeld
            ? Mathf.Min(this.SprintMultiplier + (5.0f * Time.deltaTime), Setting.FlySprintMultiplier)
            : 1.0f;

    void Move(Vector3 direction) {
        Vector3 translatedDirection = this.transform.TransformDirection(direction);
        this.LastPosition += translatedDirection * Time.deltaTime * Setting.FlySpeed * this.SprintMultiplier;
        this.transform.position = this.LastPosition;
    }
}
