using Hax;

internal class KeyboardMovement : MonoBehaviour {
    const float BaseSpeed = 20;
    float SprintMultiplier { get; set; } = 1;
    internal Vector3 LastPosition { get; set; }

    void OnEnable() => this.LastPosition = this.transform.position;

    void LateUpdate() {
        if (Helper.LocalPlayer?.isTypingChat is not false) return;

        Vector3 direction = new(
            Keyboard.current.dKey.ReadValue() - Keyboard.current.aKey.ReadValue(),
            Keyboard.current.spaceKey.ReadValue() - Keyboard.current.ctrlKey.ReadValue(),
            Keyboard.current.wKey.ReadValue() - Keyboard.current.sKey.ReadValue()
        );

        this.UpdateSprintMultiplier(Keyboard.current);
        this.Move(direction);
    }

    void UpdateSprintMultiplier(Keyboard keyboard) =>
        this.SprintMultiplier = keyboard.shiftKey.IsPressed()
            ? Mathf.Min(this.SprintMultiplier + (5.0f * Time.deltaTime), 5.0f)
            : 1.0f;

    void Move(Vector3 direction) {
        Vector3 translatedDirection =
            (this.transform.right * direction.x) +
            (this.transform.up * direction.y) +
            (this.transform.forward * direction.z);

        this.LastPosition += translatedDirection * Time.deltaTime * KeyboardMovement.BaseSpeed * this.SprintMultiplier;
        this.transform.position = this.LastPosition;
    }
}
