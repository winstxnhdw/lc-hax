using Hax;

enum CentipedeAiState {
    SEARCHING = 0,
    HIDING = 1,
    CHASING = 2,
    CLINGING = 3
}

internal class SnareFleaController : IEnemyController<CentipedeAI> {
    bool IsClingingToSomething(CentipedeAI enemy) {
        Reflector centipedeReflector = enemy.Reflect();

        return enemy.clingingToPlayer is not null || enemy.inSpecialAnimation ||
               centipedeReflector.GetInternalField<bool>("clingingToDeadBody") ||
               centipedeReflector.GetInternalField<bool>("clingingToCeiling") ||
               centipedeReflector.GetInternalField<bool>("startedCeilingAnimationCoroutine") ||
               centipedeReflector.GetInternalField<bool>("inDroppingOffPlayerAnim");
    }

    public void UsePrimarySkill(CentipedeAI enemy) {
        if (enemy.currentBehaviourStateIndex is not 1) return;
        enemy.SwitchToBehaviourServerRpc(2);
    }

    public void UseSecondarySkill(CentipedeAI enemy) {
        if (this.IsClingingToSomething(enemy)) return;

        _ = enemy.Reflect().InvokeInternalMethod("RaycastToCeiling");
        enemy.SetBehaviourState(CentipedeAiState.CHASING);
    }

    public bool IsAbleToMove(CentipedeAI enemy) => !this.IsClingingToSomething(enemy);

    public string GetPrimarySkillName(CentipedeAI _) => "Drop";

    public string GetSecondarySkillName(CentipedeAI _) => "Attach to ceiling";

    public float InteractRange(CentipedeAI _) => 1.5f;

    public bool CanUseEntranceDoors(CentipedeAI _) => false;

    public bool SyncAnimationSpeedEnabled(CentipedeAI _) => false;
}
