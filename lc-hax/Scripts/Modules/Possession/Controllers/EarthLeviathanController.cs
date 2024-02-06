internal class EarthLeviathanController : IEnemyController<SandWormAI> {
    bool IsEmerged(SandWormAI instance) => instance.inEmergingState || instance.emerged;

    internal void UseSecondarySkill(SandWormAI enemyInstance) {
        if (this.IsEmerged(enemyInstance)) return;
        enemyInstance.StartEmergeAnimation();
    }

    internal string GetSecondarySkillName(SandWormAI _) => "Emerge";
}
