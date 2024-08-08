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
        float targetCameraOffsetY = 2.3f;
        float targetCameraOffsetZ = -3.0f;

        if (enemy.IsBehaviourState(ManticoilState.GLIDING)) { // In Air
            transitionSpeed = 1.2f;
            targetCameraOffsetY = 22.5f;
            targetCameraOffsetZ = -3.5f;
        }

        float transitionSpeedDelta = transitionSpeed * Time.deltaTime;
        cameraOffset.y = Mathf.Lerp(cameraOffset.y, targetCameraOffsetY, transitionSpeedDelta);
        cameraOffset.z = Mathf.Lerp(cameraOffset.z, targetCameraOffsetZ, transitionSpeedDelta);

        return cameraOffset;
    }

    public void UsePrimarySkill(DoublewingAI enemy) => enemy.SetBehaviourState(ManticoilState.IDLE);

    public void UseSecondarySkill(DoublewingAI enemy) => enemy.SetBehaviourState(ManticoilState.GLIDING);

    public bool IsAbleToRotate(DoublewingAI enemy) => !enemy.IsBehaviourState(ManticoilState.IDLE);

    public bool SyncAnimationSpeedEnabled(DoublewingAI enemy) => true;

}

