internal class EarthLeviathanController : IEnemyController<SandWormAI> {
    bool IsEmerged(SandWormAI instance) => instance.inEmergingState || instance.emerged;

    public void UseSecondarySkill(SandWormAI enemyInstance) {
        if (this.IsEmerged(enemyInstance)) return;
        enemyInstance.StartEmergeAnimation();
    }

    public string GetSecondarySkillName(SandWormAI _) => "Emerge";

    public bool CanUseEntranceDoors(SandWormAI _) => false;

    public float InteractRange(SandWormAI _) => 0.0f;

    public bool SyncAnimationSpeedEnabled(SandWormAI _) => false;

}
