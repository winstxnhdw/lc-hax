using Hax;
using UnityEngine;

enum BrackenState {
    SCOUTING,
    STAND,
    ANGER
}

internal class BrackenController : IEnemyController<FlowermanAI> {
    float TransitionSpeed { get; set; } = 0.0f;
    Vector3 CameraOffset { get; set; } = new Vector3(0.0f, 2.0f, -3.0f);

    public Vector3 GetCameraOffset(FlowermanAI enemy) {
        this.TransitionSpeed = 4.0f;
        float targetCamOffsetY = 2.6f;
        float targetCamOffsetZ = -3.2f;

        if (enemy.IsBehaviourState(BrackenState.SCOUTING)) {
            this.TransitionSpeed = 4.0f;
            targetCamOffsetY = 2.0f;
            targetCamOffsetZ = -3.0f;
        }

        this.CameraOffset = new Vector3(
            this.CameraOffset.x,
            Mathf.Lerp(this.CameraOffset.y, targetCamOffsetY, Time.deltaTime * this.TransitionSpeed),
            Mathf.Lerp(this.CameraOffset.z, targetCamOffsetZ, Time.deltaTime * this.TransitionSpeed)
        );

        return this.CameraOffset;
    }

    public void UsePrimarySkill(FlowermanAI enemy) {
        if (!enemy.carryingPlayerBody) {
            enemy.SetBehaviourState(BrackenState.ANGER);
        }

        enemy.DropPlayerBodyServerRpc();
    }

    public void UseSecondarySkill(FlowermanAI enemy) => enemy.SetBehaviourState(BrackenState.STAND);

    public void ReleaseSecondarySkill(FlowermanAI enemy) => enemy.SetBehaviourState(BrackenState.SCOUTING);

    public bool IsAbleToMove(FlowermanAI enemy) => !enemy.inSpecialAnimation;

    public string GetPrimarySkillName(FlowermanAI enemy) => enemy.carryingPlayerBody ? "Drop body" : "";

    public string GetSecondarySkillName(FlowermanAI _) => "Stand";

    public bool SyncAnimationSpeedEnabled(FlowermanAI _) => false;
}
