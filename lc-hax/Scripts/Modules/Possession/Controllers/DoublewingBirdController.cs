using Hax;
using UnityEngine;

enum DoublewingBirdState {
    IDLE,
    GLIDING,
}

internal class DoublewingBirdController : IEnemyController<DoublewingAI> {
    public float transitionSpeed = 0f;

    public Vector3 CamOffsets = new(0, 2.3f, -3f);
    public Vector3 GetCameraOffset(DoublewingAI enemy) {
        float targetCamOffsetY, targetCamOffsetZ;

        if (enemy.IsBehaviourState(DoublewingBirdState.GLIDING)) { // In Air
            this.transitionSpeed = 1.2f;
            targetCamOffsetY = 22.5f;
            targetCamOffsetZ = -3.5f;
        }
        else { // On Ground
            this.transitionSpeed = 1.3f;
            targetCamOffsetY = 2.3f;
            targetCamOffsetZ = -3f;
        }

        this.CamOffsets.y = Mathf.Lerp(this.CamOffsets.y, targetCamOffsetY, Time.deltaTime * this.transitionSpeed);
        this.CamOffsets.z = Mathf.Lerp(this.CamOffsets.z, targetCamOffsetZ, Time.deltaTime * this.transitionSpeed);

        return this.CamOffsets;
    }

    public void UsePrimarySkill(DoublewingAI enemy) => enemy.SetBehaviourState(DoublewingBirdState.IDLE);

    public void UseSecondarySkill(DoublewingAI enemy) => enemy.SetBehaviourState(DoublewingBirdState.GLIDING);

    public bool IsAbleToRotate(EnemyAI enemy) => !enemy.IsBehaviourState(DoublewingBirdState.IDLE);

    public bool SyncAnimationSpeedEnabled(DoublewingAI enemy) => true;
}

