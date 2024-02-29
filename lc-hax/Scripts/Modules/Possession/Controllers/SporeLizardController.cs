using Hax;
using UnityEngine;

public enum SporeLizardState {
    IDLE,
    ALERTED,
    HOSTILE
}

internal class SporeLizardController : IEnemyController<PufferAI> {

    float GetTimeSinceHittingPlayer(PufferAI enemy) =>
        enemy.Reflect().GetInternalField<float>("timeSinceHittingPlayer");

    void SetTimeSinceHittingPlayer(PufferAI enemy, float value) =>
        enemy.Reflect().SetInternalField("timeSinceHittingPlayer", value);

    public void UsePrimarySkill(PufferAI enemy) {
        enemy.SetBehaviourState(SporeLizardState.HOSTILE);
        enemy.StompServerRpc();
    }

    public void UseSecondarySkill(PufferAI enemy) {
        enemy.SetBehaviourState(SporeLizardState.HOSTILE);
        enemy.ShakeTailServerRpc();
    }

    public string GetPrimarySkillName(PufferAI _) => "Stomp";

    public string GetSecondarySkillName(PufferAI _) => "Smoke";

    public void OnOutsideStatusChange(PufferAI enemy) => enemy.StopSearch(enemy.roamMap, true);

}
