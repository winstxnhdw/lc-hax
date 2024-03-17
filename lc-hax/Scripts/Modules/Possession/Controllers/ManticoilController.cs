using Hax;
using UnityEngine;

enum ManticoilState {
    IDLE,
    GLIDING,
}

class DoublewingBirdController : IEnemyController<DoublewingAI> {
    public Vector3 GetCameraOffset(DoublewingAI enemy) {
        Vector3 cameraOffset = new(0.0f, 2.3f, -3.0f);

        float transitionSpeed = 1.3f;
        float targetCamOffsetY = 2.3f;
        float targetCamOffsetZ = -3.0f;

        if (enemy.IsBehaviourState(ManticoilState.GLIDING)) { // In Air
            transitionSpeed = 1.2f;
            targetCamOffsetY = 22.5f;
            targetCamOffsetZ = -3.5f;
        }

        float transitionSpeedDelta = transitionSpeed * Time.deltaTime;
        cameraOffset.y = Mathf.Lerp(cameraOffset.y, targetCamOffsetY, transitionSpeedDelta);
        cameraOffset.z = Mathf.Lerp(cameraOffset.z, targetCamOffsetZ, transitionSpeedDelta);

        return cameraOffset;
    }

    public void UsePrimarySkill(DoublewingAI enemy) => enemy.SetBehaviourState(ManticoilState.IDLE);

    public void UseSecondarySkill(DoublewingAI enemy) => enemy.SetBehaviourState(ManticoilState.GLIDING);

    public bool IsAbleToRotate(DoublewingAI enemy) => !enemy.IsBehaviourState(ManticoilState.IDLE);

    public bool SyncAnimationSpeedEnabled(DoublewingAI enemy) => true;

}

