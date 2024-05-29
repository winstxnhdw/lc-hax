using UnityEngine;
using UnityEngine.InputSystem;

internal class MousePan : MonoBehaviour
{
    private float Sensitivity { get; set; } = 0.2f;
    private float Yaw { get; set; } = 0.0f;
    private float Pitch { get; set; } = 0.0f;

    private void OnEnable()
    {
        InputListener.OnLeftBracketPress += DecreaseMouseSensitivity;
        InputListener.OnRightBracketPress += IncreaseMouseSensitivity;
    }

    private void OnDisable()
    {
        InputListener.OnLeftBracketPress -= DecreaseMouseSensitivity;
        InputListener.OnRightBracketPress -= IncreaseMouseSensitivity;
    }

    private void IncreaseMouseSensitivity()
    {
        Sensitivity += 0.1f;
    }

    private void DecreaseMouseSensitivity()
    {
        Sensitivity = Mathf.Max(Sensitivity - 0.1f, 0.1f);
    }

    private void Update()
    {
        Yaw += Mouse.current.delta.x.ReadValue() * Sensitivity;
        Yaw = (Yaw + 360) % 360;

        Pitch -= Mouse.current.delta.y.ReadValue() * Sensitivity * (Setting.InvertYAxis ? -1 : 1);
        Pitch = Mathf.Clamp(Pitch, -90, 90);

        transform.localEulerAngles = new Vector3(Pitch, Yaw, 0.0f);
    }
}