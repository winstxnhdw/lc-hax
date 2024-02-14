
using Hax;

public enum PufferState {
    IDLE = 0,
    ALERTED = 1,
    HOSTILE = 2
}

internal class SporeLizardController : IEnemyController<PufferAI> {
    public void UsePrimarySkill(PufferAI enemyInstance) {
        enemyInstance.SetBehaviourState(PufferState.HOSTILE);
        enemyInstance.StompServerRpc();
    }

    public void UseSecondarySkill(PufferAI enemyInstance) {
        enemyInstance.SetBehaviourState(PufferState.HOSTILE);
        enemyInstance.ShakeTailServerRpc();
    }

    public string GetPrimarySkillName(PufferAI _) => "Stomp";

    public string GetSecondarySkillName(PufferAI _) => "Smoke";

    public float? InteractRange(PufferAI _) => 2.5f;
}
