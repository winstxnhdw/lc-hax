using UnityEngine;
using GameNetcodeStuff;
using UnityEngine.InputSystem;
using Hax;
using System.Linq;

class CharacterMovement : MonoBehaviour {

    // Movement constants
    const float WalkingSpeed = 0.5f; // Walking speed when left control is held
    const float SprintDuration = 0.0f; // Duration sprint key must be held for sprinting (adjust as needed)
    const float JumpForce = 9.2f;
    const float Gravity = 18.0f;

    internal float CharacterSpeed { get; set; } = 5.0f;
    internal float CharacterSprintSpeed { get; set; } = 2.8f;

    internal bool CanMove { get; set; } = true;

    // used to sync with the enemy to make sure it plays the correct animation when it is moving
    internal bool IsMoving { get; private set; } = false;
    internal bool IsSprinting { get; private set; } = false;

    // Components and state variables
    float VelocityY { get; set; } = 0.0f;
    bool IsSprintHeld { get; set; } = false;
    float SprintTimer { get; set; } = 0.0f;
    Keyboard Keyboard { get; set; } = Keyboard.current;
    KeyboardMovement? NoClipKeyboard { get; set; } = null;
    CharacterController? CharacterController { get; set; }

    void OnEnable() {
        if (this.CharacterController is not null)
            this.CharacterController.enabled = true;
    }
    void OnDisable(){
        if (this.CharacterController is not null)
            this.CharacterController.enabled = false;
    }

    void OnDestroy() {
        Destroy(this.CharacterController);
        Destroy(this.NoClipKeyboard);
    }

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

    internal void CalibrateCollision(GrabbableObject scrap) {
        if (this.CharacterController is null) return;

        this.CharacterController.height = 1.0f;
        this.CharacterController.radius = 0.5f;
        this.CharacterController.center = new Vector3(0.0f, 0.5f, 0.0f);

        float maxStepOffset = 0.25f;
        this.CharacterController.stepOffset = Mathf.Min(this.CharacterController.stepOffset, maxStepOffset);

        scrap.GetComponentsInChildren<Collider>()
            .Where(collider => collider != this.CharacterController)
            .ForEach(collider => Physics.IgnoreCollision(this.CharacterController, collider));
    }

    void Awake() {
        this.Keyboard = Keyboard.current;
        this.NoClipKeyboard = this.gameObject.GetOrAddComponent<KeyboardMovement>();
        this.CharacterController = this.gameObject.GetOrAddComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update() {
        if (this.NoClipKeyboard is { enabled: true }) return;
        if (this.CharacterController is { enabled: false }) return;

        Vector2 moveInput = new Vector2(
            this.Keyboard.dKey.ReadValue() - this.Keyboard.aKey.ReadValue(),
            this.Keyboard.wKey.ReadValue() - this.Keyboard.sKey.ReadValue()
        ).normalized;

        this.IsMoving = moveInput.magnitude > 0.0f;

        float speedModifier = this.Keyboard.leftCtrlKey.isPressed
            ? CharacterMovement.WalkingSpeed
            : 1.0f;

        // Calculate movement direction relative to character's forward direction
        Vector3 forward = Vector3.ProjectOnPlane(this.transform.forward, Vector3.up);
        Vector3 right = Vector3.ProjectOnPlane(this.transform.right, Vector3.up);
        Vector3 moveDirection = (forward * moveInput.y) + (right * moveInput.x);

        // Apply speed and sprint modifiers
        moveDirection *= speedModifier * (
            this.IsSprinting
                ? this.CharacterSpeed * this.CharacterSprintSpeed
                : this.CharacterSpeed
        );

        // Apply gravity
        this.ApplyGravity();

        if (Helper.LocalPlayer is { isTypingChat: true }) return;
        if(!this.CanMove) return;

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
        this.VelocityY = this.CharacterController is { isGrounded: false }
            ? this.VelocityY - (CharacterMovement.Gravity * Time.deltaTime)
            : 0.0f;

        Vector3 motion = new(0.0f, this.VelocityY, 0.0f);
        _ = this.CharacterController?.Move(motion * Time.deltaTime);
    }

    // Jumping action
    void Jump() => this.VelocityY = CharacterMovement.JumpForce;
}
