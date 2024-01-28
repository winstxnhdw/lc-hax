using UnityEngine;
using GameNetcodeStuff;
using UnityEngine.InputSystem;
using System.Collections.Generic;

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

        // Components and state variables
        CharacterController characterController;
        Vector3 velocity = Vector3.zero;
        bool isSprinting = false;
        bool isSprintHeld = false;
        float sprintTimer = 0f;
        Keyboard keyboard;

        // Constructor
        public RigidbodyMovement() {
            characterController = GetComponent<CharacterController>();
            keyboard = Keyboard.current;
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
            characterController = gameObject.AddComponent<CharacterController>();

            transform.localScale = new Vector3(adjustedWidth, transform.localScale.y, adjustedDepth);
            characterController.height = 0.0f; // Adjust as needed
            characterController.center = new Vector3(0f, 0.3f, 0.5f); // Adjust as needed

            keyboard = Keyboard.current;
        }

        // Update is called once per frame
        void Update() {
            // Read movement input from keyboard
            Vector2 moveInput = new Vector2(keyboard.dKey.ReadValue() - keyboard.aKey.ReadValue(),
                                            keyboard.wKey.ReadValue() - keyboard.sKey.ReadValue()).normalized;

            // Apply speed modifier based on left control key
            float speedModifier = 1.0f;
            if (keyboard.leftCtrlKey.isPressed) {
                speedModifier = controlSlowdoneMultiplier;
            }

            // Calculate movement direction relative to character's forward direction
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
            Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;
            moveDirection.y = 0f; // Remove vertical component from the movement direction

            // Apply speed and sprint modifiers
            moveDirection *= isSprinting ? baseSpeed * sprintSpeedMultiplier * speedModifier : baseSpeed * speedModifier;

            // Apply gravity
            ApplyGravity();

            // Cap velocity
            CapVelocity();

            // Attempt to move
            characterController.Move(moveDirection * Time.deltaTime);

            // Jump if jump key is pressed
            if (keyboard.spaceKey.wasPressedThisFrame) {
                Jump();
            }

            // Sprinting mechanic: Hold to sprint
            if (keyboard.leftShiftKey.isPressed) {
                if (!isSprintHeld) {
                    sprintTimer = 0f;
                    isSprintHeld = true;
                }

                if (!isSprinting && sprintTimer >= sprintDuration) {
                    isSprinting = true;
                }

                sprintTimer += Time.deltaTime;
            }
            else {
                isSprintHeld = false;

                if (isSprinting) {
                    isSprinting = false;
                }
            }
        }

        // Cap the velocity magnitude
        void CapVelocity() {
            if (velocity.magnitude > maxVelocityMagnitude) {
                velocity = velocity.normalized * maxVelocityMagnitude;
            }
        }

        // Apply gravity to the character controller
        void ApplyGravity() {
            velocity.y -= gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);

            if (characterController.isGrounded) {
                // Reset vertical velocity if grounded
                velocity.y = 0f;
            }
            else {
                // Apply gravity if not grounded
                velocity.y -= gravity * Time.deltaTime;
            }
            characterController.Move(velocity * Time.deltaTime);
        }

        // Jumping action
        void Jump() {
            velocity.y = jumpForce;
        }
    }
}
