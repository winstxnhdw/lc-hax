enum SnareFleaState {
    SEARCHING,
    HIDING,
    CHASING,
    CLINGING
}

class SnareFleaController : IEnemyController<CentipedeAI> {
    static bool IsClingingToSomething(CentipedeAI enemy) {
        Reflector<CentipedeAI> centipedeReflector = enemy.Reflect();

        return enemy.clingingToPlayer is not null || enemy.inSpecialAnimation ||
               centipedeReflector.GetInternalField<bool>("clingingToDeadBody") ||
               centipedeReflector.GetInternalField<bool>("clingingToCeiling") ||
               centipedeReflector.GetInternalField<bool>("startedCeilingAnimationCoroutine") ||
               centipedeReflector.GetInternalField<bool>("inDroppingOffPlayerAnim");
    }

    public void UsePrimarySkill(CentipedeAI enemy) {
        if (!enemy.IsBehaviourState(SnareFleaState.HIDING)) return;
        enemy.SetBehaviourState(SnareFleaState.CHASING);
    }

    public void UseSecondarySkill(CentipedeAI enemy) {
        if (SnareFleaController.IsClingingToSomething(enemy)) return;
        _ = enemy.Reflect().InvokeInternalMethod("RaycastToCeiling");
        enemy.SetBehaviourState(SnareFleaState.HIDING);
    }

    public bool IsAbleToMove(CentipedeAI enemy) => !IsClingingToSomething(enemy);

    public bool IsAbleToRotate(CentipedeAI enemy) => !IsClingingToSomething(enemy);

    public string GetPrimarySkillName(CentipedeAI _) => "Drop";

    public string GetSecondarySkillName(CentipedeAI _) => "Attach to ceiling";

    public float InteractRange(CentipedeAI _) => 1.5f;

    public bool CanUseEntranceDoors(CentipedeAI _) => false;

    public bool SyncAnimationSpeedEnabled(CentipedeAI _) => false;
}
