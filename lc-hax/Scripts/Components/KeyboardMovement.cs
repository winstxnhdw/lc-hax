using UnityEngine;
using UnityEngine.InputSystem;

internal class KeyboardMovement : MonoBehaviour
{
    private const float BaseSpeed = 20;
    private float SprintMultiplier { get; set; } = 1;

    internal Vector3 LastPosition { get; set; }
    internal bool IsPaused { get; set; } = false;

    private void OnEnable()
    {
        LastPosition = transform.position;
    }

    private void LateUpdate()
    {
        if (IsPaused) return;

        Vector3 direction = new(
            Keyboard.current.dKey.ReadValue() - Keyboard.current.aKey.ReadValue(),
            Keyboard.current.spaceKey.ReadValue() - Keyboard.current.ctrlKey.ReadValue(),
            Keyboard.current.wKey.ReadValue() - Keyboard.current.sKey.ReadValue()
        );

        UpdateSprintMultiplier(Keyboard.current);
        Move(direction);
    }

    private void UpdateSprintMultiplier(Keyboard keyboard)
    {
        SprintMultiplier = keyboard.shiftKey.IsPressed()
            ? Mathf.Min(SprintMultiplier + 5.0f * Time.deltaTime, 5.0f)
            : 1.0f;
    }

    private void Move(Vector3 direction)
    {
        var translatedDirection =
            transform.right * direction.x +
            transform.up * direction.y +
            transform.forward * direction.z;

        LastPosition += translatedDirection * Time.deltaTime * BaseSpeed * SprintMultiplier;
        transform.position = LastPosition;
    }
}