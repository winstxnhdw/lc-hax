
using Hax;

public enum PufferState {
    IDLE = 0,
    ALERTED = 1,
    HOSTILE = 2
}

internal class SporeLizardController : IEnemyController<PufferAI> {
    internal void UsePrimarySkill(PufferAI enemyInstance) {
        enemyInstance.SetBehaviourState(PufferState.HOSTILE);
        enemyInstance.StompServerRpc();
    }

    internal void UseSecondarySkill(PufferAI enemyInstance) {
        enemyInstance.SetBehaviourState(PufferState.HOSTILE);
        enemyInstance.ShakeTailServerRpc();
    }

    internal string GetPrimarySkillName(PufferAI _) => "Stomp";

    internal string GetSecondarySkillName(PufferAI _) => "Smoke";
}
