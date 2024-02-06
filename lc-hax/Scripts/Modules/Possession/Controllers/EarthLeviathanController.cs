internal class EarthLeviathanController : IEnemyController<SandWormAI> {
    bool IsEmerged(SandWormAI instance) => instance.inEmergingState || instance.emerged;

    public void UseSecondarySkill(SandWormAI enemyInstance) {
        if (this.IsEmerged(enemyInstance)) return;
        enemyInstance.StartEmergeAnimation();
    }

    public CharArray GetSecondarySkillName(SandWormAI _) => "Emerge";
}
