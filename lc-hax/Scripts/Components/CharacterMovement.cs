using GameNetcodeStuff;
using UnityEngine;

[DefaultExecutionOrder(100)]
sealed class CharacterMovement : MonoBehaviour {
    const float WalkMultiplier = 0.5f;
    const float JumpForce = 9.2f;
    const float Gravity = 18.0f;

    internal float Speed { get; set; } = 5.0f;
    internal float SprintMultiplier { get; set; } = 2.8f;
    internal bool CanMove { get; set; }
    internal bool IsMoving { get; private set; }
    internal bool IsSprinting { get; private set; }

    bool IsWHeld { get; set; }
    bool IsAHeld { get; set; }
    bool IsSHeld { get; set; }
    bool IsDHeld { get; set; }
    bool IsShiftHeld { get; set; }
    bool IsCtrlHeld { get; set; }
    bool JumpRequested { get; set; }
    float VelocityY { get; set; }
    CharacterController CharacterController => this.GetComponent<CharacterController>();

    unsafe Vector3 Direction {
        get {
            bool right = this.IsDHeld;
            bool left = this.IsAHeld;
            bool forward = this.IsWHeld;
            bool backward = this.IsSHeld;

            Vector3 direction = new(
                *(byte*)&right - *(byte*)&left,
                0.0f,
                *(byte*)&forward - *(byte*)&backward
            );

            return direction.normalized;
        }
    }

    void Awake() {
        _ = this.gameObject.AddComponent<CharacterController>();

        if (Helper.LocalPlayer is PlayerControllerB localPlayer) {
            this.gameObject.layer = localPlayer.gameObject.layer;
        }
    }

    void OnEnable() {
        this.CharacterController.enabled = true;
        InputListener.OnWButtonHold += this.HoldW;
        InputListener.OnAButtonHold += this.HoldA;
        InputListener.OnSButtonHold += this.HoldS;
        InputListener.OnDButtonHold += this.HoldD;
        InputListener.OnShiftButtonHold += this.HoldShift;
        InputListener.OnCtrlButtonHold += this.HoldCtrl;
        InputListener.OnSpacePress += this.RequestJump;
    }

    void OnDisable() {
        InputListener.OnWButtonHold -= this.HoldW;
        InputListener.OnAButtonHold -= this.HoldA;
        InputListener.OnSButtonHold -= this.HoldS;
        InputListener.OnDButtonHold -= this.HoldD;
        InputListener.OnShiftButtonHold -= this.HoldShift;
        InputListener.OnCtrlButtonHold -= this.HoldCtrl;
        InputListener.OnSpacePress -= this.RequestJump;
        this.IsWHeld = false;
        this.IsAHeld = false;
        this.IsSHeld = false;
        this.IsDHeld = false;
        this.IsShiftHeld = false;
        this.IsCtrlHeld = false;
        this.JumpRequested = false;
        this.CharacterController.enabled = false;
        this.ResetMovement();
    }

    void HoldW(bool isHeld) => this.IsWHeld = isHeld;

    void HoldA(bool isHeld) => this.IsAHeld = isHeld;

    void HoldS(bool isHeld) => this.IsSHeld = isHeld;

    void HoldD(bool isHeld) => this.IsDHeld = isHeld;

    void HoldShift(bool isHeld) => this.IsShiftHeld = isHeld;

    void HoldCtrl(bool isHeld) => this.IsCtrlHeld = isHeld;

    void RequestJump() => this.JumpRequested = true;

    void ResetMovement() {
        this.IsMoving = false;
        this.IsSprinting = false;
        this.VelocityY = 0.0f;
    }

    internal void SetPosition(Vector3 newPosition) {
        CharacterController controller = this.CharacterController;
        bool wasEnabled = controller.enabled;
        this.transform.position = newPosition;
        controller.enabled = false;
        controller.enabled = wasEnabled;
        this.ResetMovement();
    }

    internal void CalibrateCollision(EnemyAI enemy) {
        CharacterController controller = this.CharacterController;
        controller.height = 1.0f;
        controller.radius = 0.5f;
        controller.center = new Vector3(0.0f, 0.5f, 0.0f);
        controller.stepOffset = Mathf.Min(controller.stepOffset, 0.25f);

        enemy.GetComponentsInChildren<Collider>()
             .ForEach(collider => Physics.IgnoreCollision(controller, collider));
    }

    void Update() {
        CharacterController controller = this.CharacterController;
        Vector3 direction = this.Direction;

        if (!this.CanMove) {
            direction = Vector3.zero;
        }

        this.IsMoving = direction.sqrMagnitude > 0.0f;
        this.IsSprinting = this.IsMoving && this.IsShiftHeld;
        float speed = this.Speed;

        if (this.IsCtrlHeld) {
            speed *= CharacterMovement.WalkMultiplier;
        }

        if (this.IsSprinting) {
            speed *= this.SprintMultiplier;
        }

        Quaternion rotation = Quaternion.Euler(0.0f, this.transform.eulerAngles.y, 0.0f);
        Vector3 motion = rotation * direction * speed;

        if (controller.isGrounded && this.VelocityY < 0.0f) {
            this.VelocityY = -2.0f;
        }

        if (this.CanMove && controller.isGrounded && this.JumpRequested) {
            this.VelocityY = CharacterMovement.JumpForce;
        }

        this.JumpRequested = false;
        this.VelocityY -= CharacterMovement.Gravity * Time.deltaTime;
        motion.y = this.VelocityY;
        CollisionFlags collision = controller.Move(motion * Time.deltaTime);

        if ((collision & CollisionFlags.Above) != 0 && this.VelocityY > 0.0f) {
            this.VelocityY = 0.0f;
        }
    }
}
