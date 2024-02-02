public static class JesterController {

    public static void UsePrimarySkill(this JesterAI instance) {
        if (instance == null) return;
        if (!instance.isInState(JesterState.Box)) instance.SetState(JesterState.Box);
    }

    public static void UseSecondarySkill(this JesterAI instance) {
        if (instance == null) return;
        if (instance.isInState(JesterState.Box)) instance.SetState(JesterState.Cranking);
    }

    public static void ReleaseSecondarySkill(this JesterAI instance) {
        if (instance == null) return;
        if (instance.isInState(JesterState.Cranking)) instance.SetState(JesterState.PopOut);
    }

    public static string GetPrimarySkillName(this JesterAI instance) {
        return "Close box";
    }

    public static string GetSecondarySkillName(this JesterAI instance) {
        return "(HOLD) Play music";
    }

    public static bool CanMove(this JesterAI instance) {
        return instance == null ? true : !instance.isInState(JesterState.Cranking);
    }


    public static void SetState(this JesterAI instance, JesterState state) {
        if (instance == null) return;
        if (!instance.isInState(state))
            instance.SwitchToBehaviourState((int)state);
    }

    public static bool isInState(this JesterAI instance, JesterState state) {
        return instance == null ? false : instance.currentBehaviourStateIndex == (int)state;
    }

    public static float initialWalkSpeed;

    public static float initialRunSpeed;

    public static bool initialControlRotation;

    public enum JesterState {
        Box = 0,
        Cranking = 1,
        PopOut = 2
    }
}
