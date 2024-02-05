public static class FlowermanController {
    public static void UseSecondarySkill(this FlowermanAI instance) {
        instance.SetState(FlowerMan.Stand);
        if (instance.currentBehaviourStateIndex != (int)FlowerMan.Stand) {
            instance.SwitchToBehaviourServerRpc((int)FlowerMan.Stand);
        }
    }

    public static void ReleaseSecondarySkill(this FlowermanAI instance) => instance.SetState(FlowerMan.Default);

    public static void UsePrimarySkill(this FlowermanAI instance) {
        if (!instance.carryingPlayerBody) return;
        _ = instance.Reflect().InvokeInternalMethod("DropPlayerBody");
        instance.DropPlayerBodyServerRpc();
    }

    public static bool CanMove(this FlowermanAI instance) => !instance.inSpecialAnimation;

    public static string GetPrimarySkillName(this FlowermanAI instance) => instance.carryingPlayerBody ? "Drop body" : "";

    public static void SetState(this FlowermanAI instance, FlowerMan state) {
        if (instance.IsInState(state)) return;
        instance.SwitchToBehaviourServerRpc((int)state);
    }

    public static bool IsInState(this FlowermanAI instance, FlowerMan state) => instance.currentBehaviourStateIndex == (int)state;

    public enum FlowerMan {
        Default = 0,
        Stand = 1
    }
}
