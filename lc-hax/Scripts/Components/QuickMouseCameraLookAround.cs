using Hax;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuickMouseCameraLookAround : MonoBehaviour {
    public float MouseSensitivity { get; private set; } = 0.25f;
    public float Yaw { get; private set; } = 0;
    public float Pitch { get; private set; } = 0;

    void Update() {
        if (!Helper.Extant(Mouse.current, out Mouse mouse)) {
            return;
        }

        this.Yaw += mouse.delta.x.ReadValue() * this.MouseSensitivity;
        this.Yaw = this.Yaw >= 360 ? this.Yaw - 360 : this.Yaw <= -360 ? this.Yaw + 360 : this.Yaw;
        this.Pitch = Mathf.Clamp(this.Pitch - (mouse.delta.y.ReadValue() * this.MouseSensitivity), -90, 90);
        this.transform.localEulerAngles = new Vector3(this.Pitch, this.Yaw, 0);
    }
}
