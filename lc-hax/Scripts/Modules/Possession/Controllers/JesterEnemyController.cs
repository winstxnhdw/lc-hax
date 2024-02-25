using Hax;
using UnityEngine;

enum JesterState {
    CLOSED,
    CRANKING,
    OPEN
}

internal class JesterController : IEnemyController<JesterAI> {
    public float transitionSpeed = 5f;

    public void GetCameraPosition(JesterAI enemy) {
        float targetCamOffsetY, targetCamOffsetZ;

        if (!enemy.IsBehaviourState(JesterState.OPEN)) {
            targetCamOffsetY = 2f;
            targetCamOffsetZ = -3f;
        }
        else {
            targetCamOffsetY = 2.3f;
            targetCamOffsetZ = -3.5f;
        }

        // Smoothly interpolate between current and target camera positions
        PossessionMod.CamOffsetY = Mathf.Lerp(PossessionMod.CamOffsetY, targetCamOffsetY, Time.deltaTime * this.transitionSpeed);
        PossessionMod.CamOffsetZ = Mathf.Lerp(PossessionMod.CamOffsetZ, targetCamOffsetZ, Time.deltaTime * this.transitionSpeed);
    }

    void SetNoPlayerChasetimer(JesterAI enemy, float value) => enemy.Reflect().SetInternalField("noPlayersToChaseTimer", value);
    bool GetinKillAnimation(JesterAI enemy) => enemy.Reflect().GetInternalField<bool>("inKillAnimation");
    public void UsePrimarySkill(JesterAI enemy) {
        enemy.SetBehaviourState(JesterState.CLOSED);
        this.SetNoPlayerChasetimer(enemy, 0.0f);
    }

    public void OnSecondarySkillHold(JesterAI enemy) {
        if (!enemy.IsBehaviourState(JesterState.CLOSED)) return;
        enemy.SetBehaviourState(JesterState.CRANKING);
    }

    public void ReleaseSecondarySkill(JesterAI enemy) {
        if (!enemy.IsBehaviourState(JesterState.CRANKING)) return;
        enemy.SetBehaviourState(JesterState.OPEN);
    }

    public void Update(JesterAI enemy, bool isAIControlled) => this.SetNoPlayerChasetimer(enemy, 100.0f);

    public void OnUnpossess(JesterAI enemy) => this.SetNoPlayerChasetimer(enemy, 5.0f);

    public bool IsAbleToMove(JesterAI enemy) => !enemy.IsBehaviourState(JesterState.CRANKING);

    public bool IsAbleToRotate(JesterAI enemy) => !enemy.IsBehaviourState(JesterState.CRANKING);

    public string GetPrimarySkillName(JesterAI _) => "Close box";

    public string GetSecondarySkillName(JesterAI _) => "(HOLD) Begin cranking";

    public float InteractRange(JesterAI _) => 1.0f;

    public void OnOutsideStatusChange(JesterAI enemy) => enemy.StopSearch(enemy.roamMap, true);
}

