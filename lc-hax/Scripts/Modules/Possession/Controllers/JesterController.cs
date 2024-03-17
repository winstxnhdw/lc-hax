using Hax;
using UnityEngine;

enum JesterState {
    CLOSED,
    CRANKING,
    OPEN
}

class JesterController : IEnemyController<JesterAI> {
    Vector3 OriginalCameraOffset { get; } = new(0.0f, 2.5f, -3.0f);
    Vector3 CameraOffset { get; set; }

    public Vector3 GetCameraOffset(JesterAI enemy) {
        float transitionSpeed = 5.0f;
        float targetCameraOffsetY = 2.3f;
        float targetCameraOffsetZ = -3.5f;

        if (!enemy.IsBehaviourState(JesterState.OPEN)) {
            targetCameraOffsetY = 2.0f;
            targetCameraOffsetZ = -3.0f;
        }

        float transitionSpeedDelta = transitionSpeed * Time.deltaTime;
        this.CameraOffset = new Vector3(
            this.CameraOffset.x,
            Mathf.Lerp(this.CameraOffset.y, targetCameraOffsetY, transitionSpeedDelta),
            Mathf.Lerp(this.CameraOffset.z, targetCameraOffsetZ, transitionSpeedDelta)
        );

        return this.CameraOffset;
    }

    void SetNoPlayerChasetimer(JesterAI enemy, float value) =>
        enemy.Reflect().SetInternalField("noPlayersToChaseTimer", value);

    public void UsePrimarySkill(JesterAI enemy) {
        enemy.SetBehaviourState(JesterState.CLOSED);
        this.SetNoPlayerChasetimer(enemy, 0.0f);
        enemy.mainCollider.isTrigger = false;
    }

    public void OnSecondarySkillHold(JesterAI enemy) {
        if (!enemy.IsBehaviourState(JesterState.CLOSED)) return;
        enemy.SetBehaviourState(JesterState.CRANKING);
    }

    public void ReleaseSecondarySkill(JesterAI enemy) {
        if (!enemy.IsBehaviourState(JesterState.CRANKING)) return;
        enemy.SetBehaviourState(JesterState.OPEN);
        enemy.mainCollider.isTrigger = true;
    }

    public void Update(JesterAI enemy, bool isAIControlled) => this.SetNoPlayerChasetimer(enemy, 100.0f);

    public void OnUnpossess(JesterAI enemy) {
        this.SetNoPlayerChasetimer(enemy, 25.0f);
        this.CameraOffset = this.OriginalCameraOffset;
    }

    public void OnPossess(JesterAI enemy) => this.CameraOffset = this.OriginalCameraOffset;

    public bool IsAbleToMove(JesterAI enemy) => !enemy.IsBehaviourState(JesterState.CRANKING);

    public bool IsAbleToRotate(JesterAI enemy) => !enemy.IsBehaviourState(JesterState.CRANKING);

    public string GetPrimarySkillName(JesterAI _) => "Close box";

    public string GetSecondarySkillName(JesterAI _) => "(HOLD) Begin cranking";

    public void OnOutsideStatusChange(JesterAI enemy) => enemy.StopSearch(enemy.roamMap, true);
}

