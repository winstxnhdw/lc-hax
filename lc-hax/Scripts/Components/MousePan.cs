using Hax;
using UnityEngine;
using UnityEngine.InputSystem;

public class MousePan : MonoBehaviour {
    const float sensitivity = 0.25f;
    float Yaw { get; set; } = 0.0f;
    float Pitch { get; set; } = 0.0f;

    void Update() {
        if (!Mouse.current.IsNotNull(out Mouse mouse)) return;

        this.Yaw += mouse.delta.x.ReadValue() * MousePan.sensitivity;
        this.Yaw = (this.Yaw + 360) % 360;

        this.Pitch = Mathf.Clamp(this.Pitch - (mouse.delta.y.ReadValue() * MousePan.sensitivity), -90, 90);
        this.transform.localEulerAngles = new Vector3(this.Pitch, this.Yaw, 0.0f);
    }
}
