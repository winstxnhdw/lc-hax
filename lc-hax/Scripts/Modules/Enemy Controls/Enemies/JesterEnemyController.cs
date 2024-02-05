public static class JesterController {

    public static void UsePrimarySkill(this JesterAI instance) {
        if (instance.IsInState(JesterState.Box)) return;
        instance.SetState(JesterState.Box);
    }

    public static void UseSecondarySkill(this JesterAI instance) {
        if (!instance.IsInState(JesterState.Box)) return;
        instance.SetState(JesterState.Cranking);
    }

    public static void ReleaseSecondarySkill(this JesterAI instance) {
        if (!instance.IsInState(JesterState.Cranking)) return;
        instance.SetState(JesterState.PopOut);
    }

    public static string GetPrimarySkillName(this JesterAI _) => "Close box";

    public static string GetSecondarySkillName(this JesterAI _) => "(HOLD) Play music";

    public static bool CanMove(this JesterAI instance) => !instance.IsInState(JesterState.Cranking);

    public static void SetState(this JesterAI instance, JesterState state) {
        if (instance.IsInState(state)) return;
        instance.SwitchToBehaviourServerRpc((int)state);
    }

    public static bool IsInState(this JesterAI instance, JesterState state) => instance.currentBehaviourStateIndex == (int)state;

    public static float initialWalkSpeed;

    public static float initialRunSpeed;

    public static bool initialControlRotation;

    public enum JesterState {
        Box = 0,
        Cranking = 1,
        PopOut = 2
    }
}
