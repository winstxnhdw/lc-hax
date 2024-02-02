public static class SpringManController {


    public static bool get_HasStopped(this SpringManAI instance) {
        return instance == null ? false : instance.Reflect().GetInternalField<bool>("hasStopped");
    }
    public static bool get_StoppingMovement(this SpringManAI instance) {
        return instance == null ? false : instance.Reflect().GetInternalField<bool>("stoppingMovement");
    }
}
