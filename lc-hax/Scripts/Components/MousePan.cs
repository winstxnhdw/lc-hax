using Hax;
using UnityEngine;
using UnityEngine.InputSystem;

public class MousePan : MonoBehaviour {
    float Sensitivity { get; set; } = 0.2f;
    float Yaw { get; set; } = 0.0f;
    float Pitch { get; set; } = 0.0f;

    void OnEnable() {
        InputListener.onLeftBracketPress += this.DecreaseMouseSensitivity;
        InputListener.onRightBracketPress += this.IncreaseMouseSensitivity;
    }

    void OnDisable() {
        InputListener.onLeftBracketPress -= this.DecreaseMouseSensitivity;
        InputListener.onRightBracketPress -= this.IncreaseMouseSensitivity;
    }

    void IncreaseMouseSensitivity() => this.Sensitivity += 0.1f;

    void DecreaseMouseSensitivity() => this.Sensitivity = Mathf.Max(this.Sensitivity - 0.1f, 0.1f);

    void Update() {
        this.Yaw += Mouse.current.delta.x.ReadValue() * this.Sensitivity;
        this.Yaw = (this.Yaw + 360) % 360;

        this.Pitch -= Mouse.current.delta.y.ReadValue() * this.Sensitivity * (Setting.InvertYAxis ? -1 : 1);
        this.Pitch = Mathf.Clamp(this.Pitch, -90, 90);

        this.transform.localEulerAngles = new Vector3(this.Pitch, this.Yaw, 0.0f);
    }
}
