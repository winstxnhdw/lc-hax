using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using Hax;
using UnityEngine;
using UnityEngine.InputSystem;

internal class CharacterMovement : MonoBehaviour
{
    // Movement constants
    private const float WalkingSpeed = 0.5f; // Walking speed when left control is held
    private const float SprintDuration = 0.0f; // Duration sprint key must be held for sprinting (adjust as needed)
    private const float JumpForce = 9.2f;
    private const float Gravity = 18.0f;

    private readonly HashSet<LayerMask> ignoredLayers = new()
    {
        Mask.PlayerRagdoll,
        Mask.Enemies,
        Mask.LineOfSight,
        //Mask.InteractableObject,
        Mask.Props
    };

    internal float CharacterSpeed { get; set; } = 5.0f;
    internal float CharacterSprintSpeed { get; set; } = 2.8f;

    internal bool CanMove { get; set; } = true;

    // Components and state variables
    private float VelocityY { get; set; } = 0.0f;
    private bool IsSprintHeld { get; set; } = false;
    private float SprintTimer { get; set; } = 0.0f;
    private Keyboard Keyboard { get; set; } = Keyboard.current;
    private KeyboardMovement? NoClipKeyboard { get; set; } = null;
    private CharacterController? CharacterController { get; set; }

    private void OnEnable()
    {
        if (CharacterController is not null)
            CharacterController.enabled = true;
    }

    private void OnDisable()
    {
        if (CharacterController is not null)
            CharacterController.enabled = false;
    }

    private void OnDestroy()
    {
        Destroy(CharacterController);
        Destroy(NoClipKeyboard);
    }

    internal void SetNoClipMode(bool enabled)
    {
        if (NoClipKeyboard is null) return;
        NoClipKeyboard.enabled = enabled;
    }

    internal void Init()
    {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        gameObject.layer = localPlayer.gameObject.layer;
    }

    internal void SetPosition(Vector3 newPosition)
    {
        if (CharacterController is null) return;

        CharacterController.enabled = false;
        transform.position = newPosition;
        CharacterController.enabled = true;
    }


    private void InitializeCharacterController()
    {
        if (gameObject.GetOrAddComponent<CharacterController>() is not CharacterController controller) return;
        CharacterController = controller;

        CharacterController.excludeLayers = ignoredLayers.ToLayerMask();
        CharacterController.center = new Vector3(0f, 0.55f, 0f);
        CharacterController.height = 0.4f;

        // ignore all player colliders
        foreach (var playerControllerB in Helper.Players)
            if (playerControllerB != null)
            {
                var list = playerControllerB.GetComponentsInChildren<Collider>().ToList();
                foreach (var collider in list) Physics.IgnoreCollision(collider, CharacterController);
            }

        foreach (var collider2 in GetComponentsInChildren<Collider>())
            Physics.IgnoreCollision(collider2, CharacterController);


        CharacterController.slopeLimit = 60f;
        CharacterController.stepOffset = 0.4f;
    }


    private void Awake()
    {
        Keyboard = Keyboard.current;
        NoClipKeyboard = gameObject.GetOrAddComponent<KeyboardMovement>();
        InitializeCharacterController();
    }

    // Update is called once per frame
    private void Update()
    {
        if (NoClipKeyboard is { enabled: true }) return;
        if (CharacterController is { enabled: false }) return;

        var moveInput = Helper.PlayerInput_Move();

        var speedModifier = Keyboard.leftCtrlKey.isPressed
            ? WalkingSpeed
            : 1.0f;

        // Calculate movement direction relative to character's forward direction
        var forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        var right = Vector3.ProjectOnPlane(transform.right, Vector3.up);
        var moveDirection = forward * moveInput.y + right * moveInput.x;

        // Apply speed and sprint modifiers
        moveDirection *= speedModifier * (
            Helper.PlayerInput_Sprint()
                ? CharacterSpeed * CharacterSprintSpeed
                : CharacterSpeed
        );

        // Apply gravity
        ApplyGravity();

        if (Helper.LocalPlayer is { isTypingChat: true }) return;
        if (!CanMove) return;

        // Attempt to move
        _ = CharacterController?.Move(moveDirection * Time.deltaTime);

        // Jump if jump key is pressed
        if (Keyboard.spaceKey.wasPressedThisFrame) Jump();
    }

    // Apply gravity to the character controller
    private void ApplyGravity()
    {
        VelocityY = CharacterController is { isGrounded: false }
            ? VelocityY - Gravity * Time.deltaTime
            : 0.0f;

        Vector3 motion = new(0.0f, VelocityY, 0.0f);
        _ = CharacterController?.Move(motion * Time.deltaTime);
    }

    // Jumping action
    private void Jump()
    {
        VelocityY = JumpForce;
    }
}