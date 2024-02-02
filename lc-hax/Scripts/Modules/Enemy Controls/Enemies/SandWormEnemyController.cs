public static class SandWormController {


    public static void UseSecondarySkill(this SandWormAI instance) {
        if (instance == null) return;
        if (!instance.IsEmerged()) {
            instance.StartEmergeAnimation();
        }
    }

    public static string GetSecondarySkillName(this SandWormAI instance) {
        return "Emerge";
    }

    public static bool IsEmerged(this SandWormAI instance) {
        if (instance == null) return false;
        return instance.inEmergingState || instance.emerged;
    }
}
