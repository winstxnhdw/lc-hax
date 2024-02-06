
using Hax;

public enum PufferState {
    Idle = 0,
    Alerted = 1,
    Hostile = 2
}

internal class SporeLizardController : IEnemyController<PufferAI> {
    internal void UsePrimarySkill(PufferAI enemyInstance) {
        enemyInstance.SetBehaviourState(PufferState.Hostile);
        enemyInstance.StompServerRpc();
    }

    internal void UseSecondarySkill(PufferAI enemyInstance) {
        enemyInstance.SetBehaviourState(PufferState.Hostile);
        enemyInstance.ShakeTailServerRpc();
    }

    internal string GetPrimarySkillName(PufferAI _) => "Stomp";

    internal string GetSecondarySkillName(PufferAI _) => "Smoke";
}
