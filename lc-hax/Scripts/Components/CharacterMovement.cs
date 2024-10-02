using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using GameNetcodeStuff;
using Hax;

class CharacterMovement : MonoBehaviour {
    const float WalkingSpeed = 0.5f;
    const float SprintDuration = 0.0f;
    const float JumpForce = 9.2f;
    const float Gravity = 18.0f;

    internal float CharacterSpeed { get; set; } = 5.0f;
    internal float CharacterSprintSpeed { get; set; } = 2.8f;
    internal bool CanMove { get; set; } = true;
    internal bool IsMoving { get; private set; } = false;
    internal bool IsSprinting { get; private set; } = false;

    float VelocityY { get; set; } = 0.0f;
    float SprintTimer { get; set; } = 0.0f;
    bool IsSprintHeld { get; set; } = false;

    Keyboard Keyboard { get; } = Keyboard.current;
    KeyboardMovement? NoClipKeyboard { get; set; }
    CharacterController? CharacterController { get; set; }

    void Awake() {
        this.NoClipKeyboard = this.gameObject.AddComponent<KeyboardMovement>();
        this.CharacterController = this.gameObject.AddComponent<CharacterController>();
    }

    void OnEnable() => InputListener.OnSpacePress += this.Jump;

    void OnDisable() => InputListener.OnSpacePress -= this.Jump;

    internal void SetNoClipMode(bool enabled) {
        if (this.NoClipKeyboard is null) return;
        this.NoClipKeyboard.enabled = enabled;
    }

    internal void Init() {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        this.gameObject.layer = localPlayer.gameObject.layer;
    }

    internal void SetPosition(Vector3 newPosition) {
        if (this.CharacterController is null) return;

        this.CharacterController.enabled = false;
        this.transform.position = newPosition;
        this.CharacterController.enabled = true;
    }

    internal void CalibrateCollision(EnemyAI enemy) {
        if (this.CharacterController is null) return;

        this.CharacterController.height = 1.0f;
        this.CharacterController.radius = 0.5f;
        this.CharacterController.center = new Vector3(0.0f, 0.5f, 0.0f);

        float maxStepOffset = 0.25f;
        this.CharacterController.stepOffset = Mathf.Min(this.CharacterController.stepOffset, maxStepOffset);

        enemy.GetComponentsInChildren<Collider>()
             .Where(collider => collider != this.CharacterController)
             .ForEach(collider => Physics.IgnoreCollision(this.CharacterController, collider));
    }

    void Move(Vector3 moveDirection) {
        if (Helper.LocalPlayer is { isTypingChat: true } || !this.CanMove) return;
        if (this.Keyboard.leftShiftKey.isPressed) {
            if (!this.IsSprintHeld) {
                this.SprintTimer = 0.0f;
                this.IsSprintHeld = true;
            }

            if (!this.IsSprinting && this.SprintTimer >= CharacterMovement.SprintDuration) {
                this.IsSprinting = true;
            }

            this.SprintTimer += Time.deltaTime;
        }

        else {
            this.IsSprintHeld = false;
            this.IsSprinting = !this.IsSprinting;
        }

        _ = this.CharacterController?.Move(moveDirection * Time.deltaTime);
    }

    Vector3 GetMovementDirection() {
        Vector2 moveInput = new(
            this.Keyboard.dKey.ReadValue() - this.Keyboard.aKey.ReadValue(),
            this.Keyboard.wKey.ReadValue() - this.Keyboard.sKey.ReadValue()
        );

        float speedModifier = this.Keyboard.leftCtrlKey.isPressed ? CharacterMovement.WalkingSpeed : 1.0f;
        float sprintModifier = this.IsSprinting ? this.CharacterSpeed * this.CharacterSprintSpeed : this.CharacterSpeed;

        return sprintModifier * (
            (this.transform.forward * moveInput.y) +
            (this.transform.right * moveInput.x * speedModifier)
        );
    }

    void Update() {
        if (this.NoClipKeyboard is { enabled: true }) return;
        if (this.CharacterController is { enabled: false }) return;

        Vector3 moveDirection = this.GetMovementDirection();
        this.IsMoving = moveDirection != Vector3.zero;
        this.ApplyGravity();
        this.Move(moveDirection);
    }

    void ApplyGravity() {
        this.VelocityY = this.CharacterController is { isGrounded: false }
            ? this.VelocityY - (CharacterMovement.Gravity * Time.deltaTime)
            : 0.0f;

        _ = this.CharacterController?.Move(
            new Vector3(0.0f, this.VelocityY, 0.0f) * Time.deltaTime
        );
    }

    void Jump() => this.VelocityY = CharacterMovement.JumpForce;
}
