public static class MouthDogController {

    public static void UseSecondarySkill(this MouthDogAI instance) {
        if (instance.IsLunging()) return;
        instance.Lunge();
    }

    public static string GetSecondarySkillName(this MouthDogAI _) => "Lunge";

    public static void Lunge(this MouthDogAI instance) {
        if (instance == null) return;

        instance.SwitchToBehaviourServerRpc(2);
    }

    public static bool IsLunging(this MouthDogAI instance) => instance != null && instance.currentBehaviourStateIndex == 2;
}
