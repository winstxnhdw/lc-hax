using UnityEngine;
using GameNetcodeStuff;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace Hax;

public class RBKeyboardMovement : MonoBehaviour {
    const float baseSpeed = 25;
    const float jumpForce = 12f;
    float SprintMultiplier { get; set; } = 1;
    Rigidbody? rb = null;
    SphereCollider? sphereCollider = null;
    List<Collider> CollidedColliders { get; } = [];

    void Awake() {
        this.rb = this.gameObject.AddComponent<Rigidbody>();
        this.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        this.rb.constraints = RigidbodyConstraints.FreezeRotation;
        this.sphereCollider = this.gameObject.AddComponent<SphereCollider>();
        this.sphereCollider.radius = 0.25f;
    }

    public void Init() {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) {
            return;
        }

        this.gameObject.layer = localPlayer.gameObject.layer;
    }

    void OnEnable() {
        if (!this.rb.IsNotNull(out Rigidbody rb)) return;

        rb.isKinematic = false;
    }

    void OnDisable() {
        if (!this.rb.IsNotNull(out Rigidbody rb)) return;

        rb.isKinematic = true;
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

        if (keyboard.spaceKey.wasPressedThisFrame) this.Jump();
        if (keyboard.spaceKey.isPressed) this.BHop();
    }

    void UpdateSprintMultiplier(Keyboard keyboard) {
        this.SprintMultiplier = keyboard.shiftKey.IsPressed() ? Mathf.Min(this.SprintMultiplier + (5 * Time.deltaTime), 5) : 1;
    }

    void Move(Vector3 direction) {
        if (!this.rb.IsNotNull(out Rigidbody rb)) return;

        Vector3 forward = this.transform.forward;
        Vector3 right = this.transform.right;

        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        Vector3 translatedDirection =
            (right * direction.x) +
            (forward * direction.z);

        rb.velocity += translatedDirection * Time.deltaTime * baseSpeed * this.SprintMultiplier;
    }

    void Jump() {
        if (!this.rb.IsNotNull(out Rigidbody rb)) return;

        Vector3 newVelocity = rb.velocity;
        newVelocity.y = jumpForce;
        rb.velocity = newVelocity;
    }

    void BHop() {
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
