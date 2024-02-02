public static class FlowermanController {

    public static void UseSecondarySkill(this FlowermanAI instance) {
        if (instance == null) return;
        instance.SetState(FlowerMan.Stand);
        if (instance.currentBehaviourStateIndex != (int)FlowerMan.Stand) {
            instance.SwitchToBehaviourServerRpc((int)FlowerMan.Stand);
        }
    }

    public static void ReleaseSecondarySkill(this FlowermanAI instance) {
        if (instance == null) return;
        instance.SetState(FlowerMan.Default);
    }

    public static void UsePrimarySkill(this FlowermanAI instance) {
        if (instance == null) return;
        if (instance.carryingPlayerBody) {
            _ = instance.Reflect().InvokeInternalMethod("DropPlayerBody");
            instance.DropPlayerBodyServerRpc();
        }
    }

    public static bool CanMove(this FlowermanAI instance) {
        return instance == null || !instance.inSpecialAnimation;
    }


    public static string GetPrimarySkillName(this FlowermanAI instance) {
        return instance.carryingPlayerBody ? "Drop body" : "";
    }

    public static void SetState(this FlowermanAI instance, FlowerMan state) {
        if (instance == null) return;
        if (!instance.isInState(state))
            instance.SwitchToBehaviourServerRpc((int)state);
    }

    public static bool isInState(this FlowermanAI instance, FlowerMan state) {
        return instance != null && instance.currentBehaviourStateIndex == (int)state;
    }



    public enum FlowerMan {
        Default = 0,
        Stand = 1
    }
}
