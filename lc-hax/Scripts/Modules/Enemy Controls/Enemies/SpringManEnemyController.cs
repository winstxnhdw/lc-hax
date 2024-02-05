public static class SpringManController {
    public static bool HasStopped(this SpringManAI instance) => instance != null && instance.Reflect().GetInternalField<bool>("hasStopped");
    public static bool StopMoving(this SpringManAI instance) => instance != null && instance.Reflect().GetInternalField<bool>("stoppingMovement");
}
