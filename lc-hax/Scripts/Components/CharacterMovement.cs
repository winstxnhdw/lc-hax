using UnityEngine;
using GameNetcodeStuff;
using UnityEngine.InputSystem;
using Hax;

internal class CharacterMovement : MonoBehaviour {
    // Movement constants
    const float BaseSpeed = 5.0f;
    const float SprintSpeedMultiplier = 2.8f; // Multiplier for sprinting speed
    const float WalkingSpeed = 0.5f; // Walking speed when left control is held
    const float SprintDuration = 0.0f; // Duration sprint key must be held for sprinting (adjust as needed)
    const float JumpForce = 9.2f;
    const float Gravity = 18.0f;

    // used to sync with the enemy to make sure it plays the correct animation when it is moving
    public bool IsMoving { get; private set; } = false;
    public bool IsSprinting { get; private set; } = false;

    // Components and state variables
    CharacterController? CharacterController { get; set; }
    float VelocityY { get; set; } = 0.0f;
    bool IsSprintHeld { get; set; } = false;
    float SprintTimer { get; set; } = 0.0f;
    Keyboard Keyboard { get; set; } = Keyboard.current;
    KeyboardMovement? NoClipKeyboard { get; set; } = null;

    internal void SetNoClipMode(bool enabled) => this.NoClipKeyboard!.enabled = enabled;

    // Initialize method
    internal void Init() {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        this.gameObject.layer = localPlayer.gameObject.layer;
    }

    void Awake() {
        this.Keyboard = Keyboard.current;
        this.NoClipKeyboard = this.gameObject.AddComponent<KeyboardMovement>();
        this.CharacterController = this.gameObject.AddComponent<CharacterController>();
        this.CharacterController.height = 0.0f; // Adjust as needed
        this.CharacterController.center = new Vector3(0.0f, 0.3f, 0.5f); // Adjust as needed
        this.transform.localScale = new Vector3(0.0f, 0.0f, -0.1f);
    }

    // Update is called once per frame
    void Update() {
        if (this.NoClipKeyboard!.enabled) return;

        // Read movement input from keyboard
        Vector2 moveInput = new Vector2(
            this.Keyboard.dKey.ReadValue() - this.Keyboard.aKey.ReadValue(),
            this.Keyboard.wKey.ReadValue() - this.Keyboard.sKey.ReadValue()
        ).normalized;

        this.IsMoving = moveInput.magnitude > 0.0f;

        // Apply speed modifier based on left control key
        float speedModifier = 1.0f;

        if (this.Keyboard.leftCtrlKey.isPressed) {
            speedModifier = WalkingSpeed;
        }

        // Calculate movement direction relative to character's forward direction
        Vector3 forward = Vector3.ProjectOnPlane(this.transform.forward, Vector3.up);
        Vector3 right = Vector3.ProjectOnPlane(this.transform.right, Vector3.up);
        Vector3 moveDirection = (forward * moveInput.y) + (right * moveInput.x);

        // Apply speed and sprint modifiers
        moveDirection *= speedModifier * (
            this.IsSprinting
                ? CharacterMovement.BaseSpeed * CharacterMovement.SprintSpeedMultiplier
                : CharacterMovement.BaseSpeed
            );

        // Apply gravity
        this.ApplyGravity();

        // Attempt to move
        _ = this.CharacterController?.Move(moveDirection * Time.deltaTime);

        // Jump if jump key is pressed
        if (this.Keyboard.spaceKey.wasPressedThisFrame) {
            this.Jump();
        }

        // Sprinting mechanic: Hold to sprint
        if (this.Keyboard.leftShiftKey.isPressed) {
            if (!this.IsSprintHeld) {
                this.SprintTimer = 0f;
                this.IsSprintHeld = true;
            }

            if (!this.IsSprinting && this.SprintTimer >= CharacterMovement.SprintDuration) {
                this.IsSprinting = true;
            }

            this.SprintTimer += Time.deltaTime;
        }

        else {
            this.IsSprintHeld = false;

            if (this.IsSprinting) {
                this.IsSprinting = false;
            }
        }
    }

    // Apply gravity to the character controller
    void ApplyGravity() {
        this.VelocityY = this.CharacterController?.isGrounded is not true
            ? this.VelocityY - (CharacterMovement.Gravity * Time.deltaTime)
            : 0.0f;

        Vector3 motion = new(0.0f, this.VelocityY, 0.0f);
        _ = this.CharacterController?.Move(motion * Time.deltaTime);
    }

    // Jumping action
    void Jump() => this.VelocityY = CharacterMovement.JumpForce;
}
