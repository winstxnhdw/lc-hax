public static class SandWormController {
    public static void UseSecondarySkill(this SandWormAI instance) {
        if (instance.IsEmerged()) return;

        instance.StartEmergeAnimation();
    }

    public static string GetSecondarySkillName(this SandWormAI _) {
        return "Emerge";
    }

    public static bool IsEmerged(this SandWormAI instance) {
        return instance.inEmergingState || instance.emerged;
    }
}
