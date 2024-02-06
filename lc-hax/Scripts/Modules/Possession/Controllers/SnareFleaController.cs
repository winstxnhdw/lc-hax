public class SnareFleaController : IEnemyController<CentipedeAI> {
    public bool IsClingingToSomething(CentipedeAI enemyInstance) {
        Reflector centipedeReflector = enemyInstance.Reflect();

        return enemyInstance.clingingToPlayer != null || enemyInstance.inSpecialAnimation ||
               centipedeReflector.GetInternalField<bool>("clingingToDeadBody") ||
               centipedeReflector.GetInternalField<bool>("clingingToCeiling") ||
               centipedeReflector.GetInternalField<bool>("startedCeilingAnimationCoroutine") ||
               centipedeReflector.GetInternalField<bool>("inDroppingOffPlayerAnim");
    }

    public void UsePrimarySkill(CentipedeAI enemyInstance) {
        if (enemyInstance.currentBehaviourStateIndex is not 1) return;
        enemyInstance.SwitchToBehaviourServerRpc(2);
    }

    public void UseSecondarySkill(CentipedeAI enemyInstance) {
        if (this.IsClingingToSomething(enemyInstance)) return;

        _ = enemyInstance.Reflect().InvokeInternalMethod("RaycastToCeiling");
        enemyInstance.SwitchToBehaviourServerRpc(2);
    }

    public bool IsAbleToMove(CentipedeAI enemyInstance) => !this.IsClingingToSomething(enemyInstance);

    public string GetPrimarySkillName(CentipedeAI _) => "Drop";

    public string GetSecondarySkillName(CentipedeAI _) => "Attach to ceiling";
}
