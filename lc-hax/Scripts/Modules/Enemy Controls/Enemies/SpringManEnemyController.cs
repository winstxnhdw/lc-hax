public static class SpringManController {
    public static bool HasStopped(this SpringManAI instance) {
        return instance != null && instance.Reflect().GetInternalField<bool>("hasStopped");
    }
    public static bool StopMoving(this SpringManAI instance) {
        return instance != null && instance.Reflect().GetInternalField<bool>("stoppingMovement");
    }

    public static void SetStopMoving(this SpringManAI instance, bool value) {
        _ = instance.Reflect().SetInternalField("stoppingMovement", value);
    }

    public static void SetHasStopped(this SpringManAI instance, bool value) {
        _ = instance.Reflect().SetInternalField("hasStopped", value);
    }
    public static void SetCurrentSpeed(this SpringManAI instance, float speed) {
        _ = instance.Reflect().SetInternalField("currentChaseSpeed", speed);
    }

    public static float GetCurrentSpeed(this SpringManAI instance) {
        return instance.Reflect().GetInternalField<float>("currentChaseSpeed");
    }

    public static void SetState(this SpringManAI instance, SpringMan state) {
        if (instance.IsInState(state)) return;
        instance.SwitchToBehaviourServerRpc((int)state);
    }

    public static bool IsInState(this SpringManAI instance, SpringMan state) {
        return instance.currentBehaviourStateIndex == (int)state;
    }

    public static void OnMoving(this SpringManAI instance, bool isMoving = false) {
        instance.SetState(isMoving ? SpringMan.Chase : SpringMan.Idle);
        instance.SetStopMoving(!isMoving);
        instance.SetHasStopped(!isMoving);
    }


    public enum SpringMan {
        Idle = 0,
        Chase = 1
    }


}
