using UnityEngine;
using UnityEngine.InputSystem;

public class QuickKeyboardMoveAround : MonoBehaviour {
    public float speedMultiplier = 20;
    public bool onlyXZPlaneMovement = false;
    Vector3 direction;

    float sprintMultiplier = 1;

    // Update is called once per frame
    void Update() {
        this.direction = Vector3.zero;

        var keyboard = Keyboard.current;

        if (keyboard == null) return;

        if (keyboard.shiftKey.IsPressed()) {
            this.sprintMultiplier += 5 * Time.deltaTime;
        }
        else {
            this.sprintMultiplier = 1;
        }

        this.direction.z += keyboard.wKey.ReadValue();
        this.direction.z -= keyboard.sKey.ReadValue();
        this.direction.x += keyboard.dKey.ReadValue();
        this.direction.x -= keyboard.aKey.ReadValue();

        if (this.onlyXZPlaneMovement) {
            //ref:https://www.youtube.com/watch?v=7kGCrq1cJew
            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;

            forward.y = 0;
            right.y = 0;
            forward = forward.normalized;
            right = right.normalized;

            this.transform.position +=
                forward * this.direction.z * Time.deltaTime * this.speedMultiplier * this.sprintMultiplier
                + right * this.direction.x * Time.deltaTime * this.speedMultiplier * this.sprintMultiplier;
        }
        else {
            this.transform.position +=
                this.transform.forward * this.direction.z * Time.deltaTime * this.speedMultiplier * this.sprintMultiplier
                + this.transform.right * this.direction.x * Time.deltaTime * this.speedMultiplier * this.sprintMultiplier;
        }
    }
}
