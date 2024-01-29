using UnityEngine;
using GameNetcodeStuff;
using UnityEngine.InputSystem;

namespace Hax {
    public class RigidbodyMovement : MonoBehaviour {
        // Movement constants
        const float baseSpeed = 5.0f;
        const float sprintSpeedMultiplier = 2.8f; // Multiplier for sprinting speed
        const float controlSlowdoneMultiplier = 0.5f; // Slowdown multiplier when left control is held
        const float sprintDuration = 0.0f; // Duration sprint key must be held for sprinting (adjust as needed)
        const float jumpForce = 6.5f;
        const float gravity = 7.0f;
        const float maxVelocityMagnitude = 12.5f; // Adjust as needed
        bool isGravityEnabled = true; // Declare and initialize isGravityEnabled variable

        // Components and state variables
        CharacterController characterController;
        Vector3 velocity = Vector3.zero;
        bool isSprinting = false;
        bool isSprintHeld = false;
        float sprintTimer = 0f;
        Keyboard keyboard;

        // Constructor
        public RigidbodyMovement() {
            this.characterController = this.GetComponent<CharacterController>();
            this.keyboard = Keyboard.current;
        }

        // Initialize method
        public void Init() {
            if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
            this.gameObject.layer = localPlayer.gameObject.layer;
        }

        // Adjust collision box in Awake
        const float adjustedWidth = 0.0f; // Adjust as needed
        const float adjustedHeight = 0.0f; // Adjust as needed
        const float adjustedDepth = -0.5f; // Adjust as needed
        void Awake() {
            this.characterController = this.gameObject.AddComponent<CharacterController>();

            this.transform.localScale = new Vector3(adjustedWidth, this.transform.localScale.y, adjustedDepth);
            this.characterController.height = 0.0f; // Adjust as needed
            this.characterController.center = new Vector3(0f, 0.3f, 0.5f); // Adjust as needed

            this.keyboard = Keyboard.current;
        }

        // Update is called once per frame
        void Update() {
            // Read movement input from keyboard
            Vector2 moveInput = new Vector2(this.keyboard.dKey.ReadValue() - this.keyboard.aKey.ReadValue(),
                                            this.keyboard.wKey.ReadValue() - this.keyboard.sKey.ReadValue()).normalized;

            // Apply speed modifier based on left control key
            float speedModifier = 1.0f;
            if (this.keyboard.leftCtrlKey.isPressed) {
                speedModifier = controlSlowdoneMultiplier;
            }

            // Calculate movement direction relative to character's forward direction
            Vector3 forward = Vector3.ProjectOnPlane(this.transform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(this.transform.right, Vector3.up).normalized;
            Vector3 moveDirection = (forward * moveInput.y) + (right * moveInput.x);
            moveDirection.y = 0f; // Remove vertical component from the movement direction

            // Apply speed and sprint modifiers
            moveDirection *= this.isSprinting ? baseSpeed * sprintSpeedMultiplier * speedModifier : baseSpeed * speedModifier;

            // Toggle gravity with Q key
            if (this.keyboard.qKey.wasPressedThisFrame) {
                this.isGravityEnabled = !this.isGravityEnabled;
            }

            // Apply gravity if enabled
            if (this.isGravityEnabled) {
                this.ApplyGravity();
            }

            // Cap velocity
            this.CapVelocity();

            // Attempt to move
            _ = this.characterController.Move(moveDirection * Time.deltaTime);

            // Jump if jump key is pressed
            if (this.keyboard.spaceKey.wasPressedThisFrame) {
                this.Jump();
            }

            // Sprinting mechanic: Hold to sprint
            if (this.keyboard.leftShiftKey.isPressed) {
                if (!this.isSprintHeld) {
                    this.sprintTimer = 0f;
                    this.isSprintHeld = true;
                }

                if (!this.isSprinting && this.sprintTimer >= sprintDuration) {
                    this.isSprinting = true;
                }

                this.sprintTimer += Time.deltaTime;
            }
            else {
                this.isSprintHeld = false;

                if (this.isSprinting) {
                    this.isSprinting = false;
                }
            }
        }

        // Cap the velocity magnitude
        void CapVelocity() {
            if (this.velocity.magnitude > maxVelocityMagnitude) {
                this.velocity = this.velocity.normalized * maxVelocityMagnitude;
            }
        }

        // Apply gravity to the character controller
        void ApplyGravity() {
            this.velocity.y -= gravity * Time.deltaTime;
            _ = this.characterController.Move(this.velocity * Time.deltaTime);

            if (this.characterController.isGrounded) {
                // Reset vertical velocity if grounded
                this.velocity.y = 0f;
            }
            else {
                // Apply gravity if not grounded
                this.velocity.y -= gravity * Time.deltaTime;
            }
            _ = this.characterController.Move(this.velocity * Time.deltaTime);
        }

        // Jumping action
        void Jump() {
            this.velocity.y = jumpForce;
        }
    }
}
