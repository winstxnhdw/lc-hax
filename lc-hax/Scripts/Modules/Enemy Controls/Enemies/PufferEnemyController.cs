public static class PufferController {

    public static void UsePrimarySkill(this PufferAI instance) {
        if (instance == null) return;
        instance.SetState(PufferState.Stomp);
        instance.StompServerRpc();
    }

    public static void UseSecondarySkill(this PufferAI instance) {
        if (instance == null) return;
        instance.SetState(PufferState.Stomp);
        instance.ShakeTailServerRpc();
    }

    public static string GetPrimarySkillName(this PufferAI instance) {
        return "Stomp";
    }

    public static string GetSecondarySkillName(this PufferAI instance) {
        return "Smoke";
    }

    public static void SetState(this PufferAI instance, PufferState state) {
        if (instance == null) return;
        if (!instance.IsInState(state))
            instance.SwitchToBehaviourState((int)state);
    }

    public static bool IsInState(this PufferAI instance, PufferState state) {
        return instance != null && instance.currentBehaviourStateIndex == (int)state;
    }

    public enum PufferState {
        Default = 0,
        Stomp = 1,
    }
}
