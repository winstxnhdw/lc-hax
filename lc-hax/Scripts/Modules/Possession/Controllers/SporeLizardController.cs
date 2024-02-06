
using Hax;

public enum PufferState {
    DEFAULT = 0,
    HOSTILE = 1,
}

public class SporeLizardController : IEnemyController<PufferAI> {
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
}
