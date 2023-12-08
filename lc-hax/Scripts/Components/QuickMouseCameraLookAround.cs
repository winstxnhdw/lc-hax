using UnityEngine;
using UnityEngine.InputSystem;

public class QuickMouseCameraLookAround : MonoBehaviour {
    public float mouseSensitivity = .25f;

    public float yaw = 0;
    public float pitch = 0;
    public bool lockCursor = false;

    private void Awake() {

        this.SetLockCursor(true);
    }

    // Update is called once per frame
    void Update() {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.ctrlKey.wasPressedThisFrame) {
            this.SetLockCursor(!this.lockCursor);
        }

        if (!this.lockCursor) return;

        var mouse = Mouse.current;
        if (mouse == null) return;

        this.yaw += mouse.delta.x.ReadValue() * this.mouseSensitivity;
        this.yaw = this.yaw >= 360 ? this.yaw - 360 : this.yaw <= -360 ? this.yaw + 360 : this.yaw;
        this.pitch = Mathf.Clamp(this.pitch - mouse.delta.y.ReadValue() * this.mouseSensitivity, -90, 90);

        this.transform.localEulerAngles = new Vector3(this.pitch, this.yaw, 0);
    }

    public void SetLockCursor(bool on) {
        this.lockCursor = on;
        Cursor.lockState = on ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
