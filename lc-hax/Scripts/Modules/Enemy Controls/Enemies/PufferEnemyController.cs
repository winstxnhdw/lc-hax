public static class PufferController {
    public static void UsePrimarySkill(this PufferAI instance) {
        instance.SetState(PufferState.Stomp);
        instance.StompServerRpc();
    }

    public static void UseSecondarySkill(this PufferAI instance) {
        instance.SetState(PufferState.Stomp);
        instance.ShakeTailServerRpc();
    }

    public static string GetPrimarySkillName(this PufferAI _) => "Stomp";

    public static string GetSecondarySkillName(this PufferAI _) => "Smoke";

    public static void SetState(this PufferAI instance, PufferState state) {
        if (instance.IsInState(state)) return;
        instance.SwitchToBehaviourServerRpc((int)state);
    }

    public static bool IsInState(this PufferAI instance, PufferState state) => instance != null && instance.currentBehaviourStateIndex == (int)state;

    public enum PufferState {
        Default = 0,
        Stomp = 1,
    }
}
