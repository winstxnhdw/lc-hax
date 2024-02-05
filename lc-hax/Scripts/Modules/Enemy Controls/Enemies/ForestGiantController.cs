public static class ForestGiantController {
    public static void UseSecondarySkill(this ForestGiantAI instance) => instance.SetState(GiantStates.Chase);

    public static void ReleaseSecondarySkill(this ForestGiantAI instance) => instance.SetState(GiantStates.Default);
    public static bool CanMove(this ForestGiantAI instance) => !instance.IsEatingPlayer();

    public static bool IsEatingPlayer(this ForestGiantAI instance) => instance.Reflect().GetInternalField<bool>("inEatingPlayerAnimation");

    public static void SetState(this ForestGiantAI instance, GiantStates state) {
        if (instance.IsInState(state)) return;
        instance.SwitchToBehaviourServerRpc(unchecked((int)state));
    }

    public static bool IsInState(this ForestGiantAI instance, GiantStates state) => instance.currentBehaviourStateIndex == unchecked((int)state);

    public enum GiantStates {
        Default = 0,
        Chase = 1
    }
}
