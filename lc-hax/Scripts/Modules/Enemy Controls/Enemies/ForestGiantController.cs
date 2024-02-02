public static class ForestGiantController {


    public static void UseSecondarySkill(this ForestGiantAI instance) {
        if (instance == null) return;
        instance.SetState(GiantStates.Chase);
    }

    public static void ReleaseSecondarySkill(this ForestGiantAI instance) {
        if (instance == null) return;

        instance.SetState(GiantStates.Default);

    }
    public static bool CanMove(this ForestGiantAI instance) {
        return instance == null || !instance.IsEatingPlayer();
    }

    public static bool IsEatingPlayer(this ForestGiantAI instance) {
        return instance != null && instance.Reflect().GetInternalField<bool>("inEatingPlayerAnimation");
    }
    public static void SetState(this ForestGiantAI instance, GiantStates state) {
        if (instance == null) return;
        if (!instance.isInState(state))
            instance.SwitchToBehaviourState((int)state);
    }

    public static bool isInState(this ForestGiantAI instance, GiantStates state) {
        return instance != null && instance.currentBehaviourStateIndex == (int)state;
    }


    public enum GiantStates {
        Default = 0,
        Chase = 1
    }
}
