
using Hax;

public enum SporeLizardState {
    IDLE,
    ALERTED,
    HOSTILE
}

class SporeLizardController : IEnemyController<PufferAI> {
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
}
