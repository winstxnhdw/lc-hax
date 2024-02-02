public static class CentipedeController {

    public static void UsePrimarySkill(this CentipedeAI instance) {
        if (instance == null) return;
        if (instance.currentBehaviourStateIndex == 1)
            instance.SwitchToBehaviourState(2);
    }

    public static void UseSecondarySkill(this CentipedeAI instance) {
        if (instance == null) return;
        if (!instance.IsClingingToSomething()) {
            instance.RaycastToCeiling();
            instance.SwitchToBehaviourState(2);
        }
    }

    public static void RaycastToCeiling(this CentipedeAI instance) {
        if (instance == null) return;
        _ = instance.Reflect().InvokeInternalMethod("RaycastToCeiling");
    }
    public static bool CanMove(this CentipedeAI instance) {
        return instance == null || !instance.IsClingingToSomething();
    }

    public static bool IsClingingToSomething(this CentipedeAI instance) {
        if (instance == null) return false;
        Reflector reflect = instance.Reflect();
        return reflect != null
&& (instance.clingingToPlayer != null
               || instance.inSpecialAnimation
               || reflect.GetInternalField<bool>("clingingToDeadBody")
               || reflect.GetInternalField<bool>("clingingToCeiling")
               || reflect.GetInternalField<bool>("startedCeilingAnimationCoroutine")
               || reflect.GetInternalField<bool>("inDroppingOffPlayerAnim"));
    }

    public static string GetPrimarySkillName(this CentipedeAI instance) {
        return "Drop";
    }

    public static string GetSecondarySkillName(this CentipedeAI instance) {
        return "Attach to ceiling";
    }

}
