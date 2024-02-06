
using Hax;

internal enum PufferState {
    DEFAULT = 0,
    HOSTILE = 1,
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
