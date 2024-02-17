
using Hax;

public enum PufferState {
    IDLE = 0,
    ALERTED = 1,
    HOSTILE = 2
}

internal class SporeLizardController : IEnemyController<PufferAI> {
    public void UsePrimarySkill(PufferAI enemy) {
        enemy.SetBehaviourState(PufferState.HOSTILE);
        enemy.StompServerRpc();
    }

    public void UseSecondarySkill(PufferAI enemy) {
        enemy.SetBehaviourState(PufferState.HOSTILE);
        enemy.ShakeTailServerRpc();
    }

    public string GetPrimarySkillName(PufferAI _) => "Stomp";

    public string GetSecondarySkillName(PufferAI _) => "Smoke";

    public float InteractRange(PufferAI _) => 2.5f;
}
