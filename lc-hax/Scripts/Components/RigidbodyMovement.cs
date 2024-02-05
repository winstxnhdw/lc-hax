using UnityEngine;
using GameNetcodeStuff;
using UnityEngine.InputSystem;

namespace Hax; 
public class RigidbodyMovement : MonoBehaviour {
    // Movement constants
    const float baseSpeed = 5.0f;
    const float sprintSpeedMultiplier = 2.8f; // Multiplier for sprinting speed
    const float walkingSpeed = 0.5f; // Walking speed when left control is held
    const float sprintDuration = 0.0f; // Duration sprint key must be held for sprinting (adjust as needed)
    const float jumpForce = 6.5f;
    const float gravity = 10.0f;
    const float maxVelocityMagnitude = 12.5f; // Adjust as needed

    // Components and state variables
    CharacterController CharacterController { get; set; }
    float VelocityY { get; set; } = 0.0f;
    bool IsSprinting { get; set; } = false;
    bool IsSprintHeld { get; set; } = false;
    float SprintTimer { get; set; } = 0.0f;
    Keyboard Keyboard { get; set; } = Keyboard.current;

    // Adjust collision box in Awake
    const float adjustedWidth = 0.0f; // Adjust as needed
    const float adjustedHeight = 0.0f; // Adjust as needed
    const float adjustedDepth = -0.5f; // Adjust as needed

    public RigidbodyMovement() => this.CharacterController = this.GetComponent<CharacterController>();

    // Initialize method
    public void Init() {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        this.gameObject.layer = localPlayer.gameObject.layer;
    }

    void Awake() {
        this.CharacterController = this.gameObject.AddComponent<CharacterController>();

        this.transform.localScale = new Vector3(adjustedWidth, this.transform.localScale.y, adjustedDepth);
        this.CharacterController.height = 0.0f; // Adjust as needed
        this.CharacterController.center = new Vector3(0f, 0.3f, 0.5f); // Adjust as needed

        this.Keyboard = Keyboard.current;
    }

    // Update is called once per frame
    void Update() {
        // Read movement input from keyboard
        Vector2 moveInput = new Vector2(
            this.Keyboard.dKey.ReadValue() - this.Keyboard.aKey.ReadValue(),
            this.Keyboard.wKey.ReadValue() - this.Keyboard.sKey.ReadValue()
        ).normalized;

        // Apply speed modifier based on left control key
        float speedModifier = 1.0f;

        if (this.Keyboard.leftCtrlKey.isPressed) {
            speedModifier = walkingSpeed;
        }

        // Calculate movement direction relative to character's forward direction
        Vector3 forward = Vector3.ProjectOnPlane(this.transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(this.transform.right, Vector3.up).normalized;
        Vector3 moveDirection = (forward * moveInput.y) + (right * moveInput.x);
        moveDirection.y = 0.0f; // Remove vertical component from the movement direction

        // Apply speed and sprint modifiers
        moveDirection *= (this.IsSprinting ? baseSpeed * sprintSpeedMultiplier : baseSpeed) * speedModifier;

        // Apply gravity
        this.ApplyGravity();

        // Attempt to move
        _ = this.CharacterController.Move(moveDirection * Time.deltaTime);

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

            if (!this.IsSprinting && this.SprintTimer >= sprintDuration) {
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
        Vector3 motion = Vector3.zero;
        this.VelocityY -= gravity * Time.deltaTime;
        motion.y = this.VelocityY;
        _ = this.CharacterController.Move(motion * Time.deltaTime);
    }

    // Jumping action
    void Jump() => this.VelocityY = jumpForce;
}
