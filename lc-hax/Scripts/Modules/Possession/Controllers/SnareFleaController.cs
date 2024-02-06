enum CentipedeAiState {
    Searching = 0,
    CeilingHiding = 1,
    Chasing = 2,
    Clinging = 3
}


internal class SnareFleaController : IEnemyController<CentipedeAI> {
    internal bool IsClingingToSomething(CentipedeAI enemyInstance) {
        Reflector centipedeReflector = enemyInstance.Reflect();

        return enemyInstance.clingingToPlayer != null || enemyInstance.inSpecialAnimation ||
               centipedeReflector.GetInternalField<bool>("clingingToDeadBody") ||
               centipedeReflector.GetInternalField<bool>("clingingToCeiling") ||
               centipedeReflector.GetInternalField<bool>("startedCeilingAnimationCoroutine") ||
               centipedeReflector.GetInternalField<bool>("inDroppingOffPlayerAnim");
    }

    internal void UsePrimarySkill(CentipedeAI enemyInstance) {
        if (enemyInstance.currentBehaviourStateIndex is not 1) return;
        enemyInstance.SwitchToBehaviourServerRpc(2);
    }

    internal void UseSecondarySkill(CentipedeAI enemyInstance) {
        if (this.IsClingingToSomething(enemyInstance)) return;

        _ = enemyInstance.Reflect().InvokeInternalMethod("RaycastToCeiling");
        enemyInstance.SwitchToBehaviourServerRpc(2);
    }

    internal bool IsAbleToMove(CentipedeAI enemyInstance) => !this.IsClingingToSomething(enemyInstance);

    internal string GetPrimarySkillName(CentipedeAI _) => "Drop";

    internal string GetSecondarySkillName(CentipedeAI _) => "Attach to ceiling";
}
