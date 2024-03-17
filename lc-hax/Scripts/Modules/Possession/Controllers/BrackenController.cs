using Hax;
using UnityEngine;

enum BrackenState {
    SCOUTING,
    STAND,
    ANGER
}

class BrackenController : IEnemyController<FlowermanAI> {
    public Vector3 GetCameraOffset(FlowermanAI enemy) {
        Vector3 cameraOffset = new(0.0f, 2.0f, -3.0f);
        float transitionSpeed = 4.0f;
        float targetCamOffsetY = 2.6f;
        float targetCamOffsetZ = -3.2f;

        if (enemy.IsBehaviourState(BrackenState.SCOUTING)) {
            targetCamOffsetY = 2.0f;
            targetCamOffsetZ = -3.0f;
        }

        float transitionSpeedDelta = transitionSpeed * Time.deltaTime;
        cameraOffset.y = Mathf.Lerp(cameraOffset.y, targetCamOffsetY, transitionSpeedDelta);
        cameraOffset.z = Mathf.Lerp(cameraOffset.z, targetCamOffsetZ, transitionSpeedDelta);

        return cameraOffset;
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
