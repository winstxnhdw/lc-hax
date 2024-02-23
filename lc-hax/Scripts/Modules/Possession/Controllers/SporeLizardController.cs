
using GameNetcodeStuff;
using Hax;

public enum SporeLizardState {
    IDLE,
    ALERTED,
    HOSTILE
}

internal class SporeLizardController : IEnemyController<PufferAI> {
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

    public void OnCollideWithPlayer(PufferAI enemy, PlayerControllerB player) => enemy.OnCollideWithPlayer(player.playerCollider);
}
