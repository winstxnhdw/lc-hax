using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-200)]
sealed class MousePan : MonoBehaviour {
    internal bool IsPaused { get; set; }

    float Sensitivity { get; set; } = 0.2f;
    float Yaw { get; set; }
    float Pitch { get; set; }

    void OnEnable() {
        this.SetRotation(this.transform.rotation);
        InputListener.OnLeftBracketPress += this.DecreaseMouseSensitivity;
        InputListener.OnRightBracketPress += this.IncreaseMouseSensitivity;
    }

    void OnDisable() {
        InputListener.OnLeftBracketPress -= this.DecreaseMouseSensitivity;
        InputListener.OnRightBracketPress -= this.IncreaseMouseSensitivity;
    }

    void IncreaseMouseSensitivity() => this.Sensitivity += 0.1f;

    void DecreaseMouseSensitivity() => this.Sensitivity = Mathf.Max(this.Sensitivity - 0.1f, 0.1f);

    internal void SetRotation(Quaternion rotation) {
        this.transform.rotation = rotation;
        this.Yaw = this.transform.localEulerAngles.y;
        this.Pitch = Mathf.Clamp(Mathf.DeltaAngle(0.0f, this.transform.localEulerAngles.x), -90.0f, 90.0f);
        this.transform.localEulerAngles = new Vector3(this.Pitch, this.Yaw, 0.0f);
    }

    void Update() {
        if (this.IsPaused) return;
        if (Helper.LocalPlayer is { isTypingChat: true }) return;
        if (Helper.LocalPlayer?.quickMenuManager is { isMenuOpen: true }) return;
        if (Mouse.current is not { } mouse) return;

        this.Yaw = Mathf.Repeat(this.Yaw + (mouse.delta.x.ReadValue() * this.Sensitivity), 360.0f);
        this.Pitch -= mouse.delta.y.ReadValue() * this.Sensitivity * (Globals.InvertYAxis ? -1 : 1);
        this.Pitch = Mathf.Clamp(this.Pitch, -90, 90);

        this.transform.localEulerAngles = new Vector3(this.Pitch, this.Yaw, 0.0f);
    }
}
