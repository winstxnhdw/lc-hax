using UnityEngine;
using GameNetcodeStuff;
using UnityEngine.InputSystem;
using Hax;
using System.Linq;
using System.Collections.Generic;

class CharacterMovement : MonoBehaviour {

    // Movement constants
    const float WalkingSpeed = 0.5f; // Walking speed when left control is held
    const float SprintDuration = 0.0f; // Duration sprint key must be held for sprinting (adjust as needed)
    const float JumpForce = 9.2f;
    const float Gravity = 18.0f;

    internal float CharacterSpeed { get; set; } = 5.0f;
    internal float CharacterSprintSpeed { get; set; } = 2.8f;

    internal bool CanMove { get; set; } = true;

    // Components and state variables
    float VelocityY { get; set; } = 0.0f;
    bool IsSprintHeld { get; set; } = false;
    float SprintTimer { get; set; } = 0.0f;
    Keyboard Keyboard { get; set; } = Keyboard.current;
    KeyboardMovement? NoClipKeyboard { get; set; } = null;
    CharacterController? CharacterController { get; set; }

    readonly HashSet<LayerMask> ignoredLayers = new()
    {
        Mask.PlayerRagdoll,
        Mask.Enemies,
        Mask.LineOfSight,
        //Mask.InteractableObject,
        Mask.Props
    };
    
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


    void InitializeCharacterController() {
        if(this.gameObject.GetOrAddComponent<CharacterController>() is not CharacterController controller) return;
        this.CharacterController = controller;

        this.CharacterController.excludeLayers = ignoredLayers.ToLayerMask();
        this.CharacterController.center = new Vector3(0f, 0.55f, 0f);
        this.CharacterController.height = 0.4f;

        // ignore all player colliders
        foreach(PlayerControllerB playerControllerB in Helper.Players) {
            if (playerControllerB != null) {
                List<Collider> list = playerControllerB.GetComponentsInChildren<Collider>().ToList();
                foreach (Collider collider in list) {
                    Physics.IgnoreCollision(collider, this.CharacterController);
                }
            }
        }

        foreach (Collider collider2 in base.GetComponentsInChildren<Collider>()) {
            Physics.IgnoreCollision(collider2, this.CharacterController);
        }


        this.CharacterController.slopeLimit = 60f;
        this.CharacterController.stepOffset = 0.4f;
    }


    void Awake() {
        this.Keyboard = Keyboard.current;
        this.NoClipKeyboard = this.gameObject.GetOrAddComponent<KeyboardMovement>();
        this.InitializeCharacterController();
    }

    // Update is called once per frame
    void Update() {
        if (this.NoClipKeyboard is { enabled: true }) return;
        if (this.CharacterController is { enabled: false }) return;

        Vector2 moveInput = Helper.PlayerInput_Move();

        float speedModifier = this.Keyboard.leftCtrlKey.isPressed
            ? CharacterMovement.WalkingSpeed
            : 1.0f;

        // Calculate movement direction relative to character's forward direction
        Vector3 forward = Vector3.ProjectOnPlane(this.transform.forward, Vector3.up);
        Vector3 right = Vector3.ProjectOnPlane(this.transform.right, Vector3.up);
        Vector3 moveDirection = (forward * moveInput.y) + (right * moveInput.x);

        // Apply speed and sprint modifiers
        moveDirection *= speedModifier * (
            Helper.PlayerInput_Sprint()
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
