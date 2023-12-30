using UnityEngine;
using GameNetcodeStuff;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace Hax;

public class RigidbodyMovement : MonoBehaviour {
    const float baseSpeed = 25.0f;
    const float jumpForce = 12.0f;

    Rigidbody? rigidbody;
    SphereCollider? sphereCollider;

    float SprintMultiplier { get; set; } = 1.0f;
    List<Collider> CollidedColliders { get; } = [];

    void Awake() {
        this.rigidbody = this.gameObject.AddComponent<Rigidbody>();
        this.rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        this.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        this.sphereCollider = this.gameObject.AddComponent<SphereCollider>();
        this.sphereCollider.radius = 0.25f;
    }

    public void Init() {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) return;
        this.gameObject.layer = localPlayer.gameObject.layer;
    }

    void OnEnable() {
        if (!this.rigidbody.IsNotNull(out Rigidbody rigidbody)) return;
        rigidbody.isKinematic = false;
    }

    void OnDisable() {
        if (!this.rigidbody.IsNotNull(out Rigidbody rigidbody)) return;
        rigidbody.isKinematic = true;
    }

    void Update() {
        if (!Keyboard.current.IsNotNull(out Keyboard keyboard)) return;

        Vector3 direction = new(
            keyboard.dKey.ReadValue() - keyboard.aKey.ReadValue(),
            keyboard.spaceKey.ReadValue() - keyboard.ctrlKey.ReadValue(),
            keyboard.wKey.ReadValue() - keyboard.sKey.ReadValue()
        );

        this.UpdateSprintMultiplier(keyboard);
        this.Move(direction);

        if (keyboard.spaceKey.wasPressedThisFrame) {
            this.Jump();
        }

        if (keyboard.spaceKey.isPressed) {
            this.BunnyHop();
        }
    }

    void UpdateSprintMultiplier(Keyboard keyboard) {
        this.SprintMultiplier =
            keyboard.shiftKey.IsPressed()
                ? Mathf.Min(this.SprintMultiplier + (5.0f * Time.deltaTime), 5.0f)
                : 1.0f;
    }

    void Move(Vector3 direction) {
        if (!this.rigidbody.IsNotNull(out Rigidbody rigidbody)) return;

        Vector3 forward = this.transform.forward;
        Vector3 right = this.transform.right;

        forward.y = 0.0f;
        right.y = 0.0f;
        forward = forward.normalized;
        right = right.normalized;

        Vector3 translatedDirection = (right * direction.x) + (forward * direction.z);
        rigidbody.velocity += translatedDirection * Time.deltaTime * RigidbodyMovement.baseSpeed * this.SprintMultiplier;
    }

    void Jump() {
        if (!this.rigidbody.IsNotNull(out Rigidbody rigidbody)) return;

        Vector3 newVelocity = rigidbody.velocity;
        newVelocity.y = RigidbodyMovement.jumpForce;
        rigidbody.velocity = newVelocity;
    }

    void BunnyHop() {
        //this method covers most cases
        if (this.CollidedColliders.Count > 0) {
            this.Jump();
        }
    }

    void OnCollisionEnter(Collision collision) {
        this.CollidedColliders.Add(collision.collider);
    }

    void OnCollisionExit(Collision collision) {
        _ = this.CollidedColliders.Remove(collision.collider);
    }
}
