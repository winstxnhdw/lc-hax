using UnityEngine;

public static class SpringManController {


    public static bool get_HasStopped(this SpringManAI instance) {
        if(instance == null) return false;
        return instance.Reflect().GetInternalField<bool>("hasStopped");
    }
    public static bool get_StoppingMovement(this SpringManAI instance) {
        if (instance == null) return false;
        return instance.Reflect().GetInternalField<bool>("stoppingMovement");
    }
}
