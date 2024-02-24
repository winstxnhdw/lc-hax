
using GameNetcodeStuff;
using Hax;

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

    public float InteractRange(PufferAI _) => 2.5f;

    public void OnOutsideStatusChange(PufferAI enemy) => enemy.StopSearch(enemy.roamMap, true);

    public void OnCollideWithPlayer(PufferAI enemy, PlayerControllerB player) {
        if (enemy.isOutside) {
            if (this.GetTimeSinceHittingPlayer(enemy) > 1f) {
                this.SetTimeSinceHittingPlayer(enemy, 0f);
                player.DamagePlayer(20, true, true, CauseOfDeath.Mauling, 0, false, default);
                enemy.BitePlayerServerRpc((int)player.playerClientId);
            }
        }
    } 
}
