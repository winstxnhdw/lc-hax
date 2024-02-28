using Hax;
using UnityEngine;

enum BrackenState {
    SCOUTING,
    STAND,
    ANGER
}

internal class BrackenController : IEnemyController<FlowermanAI> {
    public float transitionSpeed = 0f;

    public Vector3 CamOffsets = new(0, 2f, -3f);
    public Vector3 GetCameraOffset(FlowermanAI enemy) {
        float targetCamOffsetY, targetCamOffsetZ;

        if (enemy.IsBehaviourState(BrackenState.SCOUTING)) {
            this.transitionSpeed = 4.0f;
            targetCamOffsetY = 2f;
            targetCamOffsetZ = -3f;
        }
        else {
            this.transitionSpeed = 4.0f;
            targetCamOffsetY = 2.6f;
            targetCamOffsetZ = -3.2f;
        }

        this.CamOffsets.y = Mathf.Lerp(this.CamOffsets.y, targetCamOffsetY, Time.deltaTime * this.transitionSpeed);
        this.CamOffsets.z = Mathf.Lerp(this.CamOffsets.z, targetCamOffsetZ, Time.deltaTime * this.transitionSpeed);

        return this.CamOffsets;
    }
    
    bool GetStartingKillAnimationLocalClient(FlowermanAI enemy) => enemy.Reflect().GetInternalField<bool>("startingKillAnimationLocalClient");

    void SetStartingKillAnimationLocalClient(FlowermanAI enemy, bool value) => enemy.Reflect().SetInternalField("startingKillAnimationLocalClient", value);


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

    public float InteractRange(FlowermanAI _) => 1.5f;

    public bool SyncAnimationSpeedEnabled(FlowermanAI _) => false;
}
