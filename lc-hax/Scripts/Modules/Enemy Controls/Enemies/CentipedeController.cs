public static class CentipedeController {
    public static void UsePrimarySkill(this CentipedeAI instance) {
        if (instance == null) return;
        if (instance.currentBehaviourStateIndex != 1) return;

        instance.SwitchToBehaviourServerRpc(2);
    }

    public static void UseSecondarySkill(this CentipedeAI instance) {
        if (instance.IsClingingToSomething()) return;

        instance.RaycastToCeiling();
        instance.SwitchToBehaviourServerRpc(2);
    }

    public static void RaycastToCeiling(this CentipedeAI instance) => _ = instance.Reflect().InvokeInternalMethod("RaycastToCeiling");
    public static bool CanMove(this CentipedeAI instance) => instance == null || !instance.IsClingingToSomething();

    public static bool IsClingingToSomething(this CentipedeAI instance) {
        Reflector centipedeReflector = instance.Reflect();

        return instance.clingingToPlayer != null
               || instance.inSpecialAnimation
               || centipedeReflector.GetInternalField<bool>("clingingToDeadBody")
               || centipedeReflector.GetInternalField<bool>("clingingToCeiling")
               || centipedeReflector.GetInternalField<bool>("startedCeilingAnimationCoroutine")
               || centipedeReflector.GetInternalField<bool>("inDroppingOffPlayerAnim");
    }

    public static string GetPrimarySkillName(this CentipedeAI _) => "Drop";

    public static string GetSecondarySkillName(this CentipedeAI _) => "Attach to ceiling";
}
