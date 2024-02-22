internal class EarthLeviathanController : IEnemyController<SandWormAI> {
    bool IsEmerged(SandWormAI enemy) => enemy.inEmergingState || enemy.emerged;

    public void UseSecondarySkill(SandWormAI enemy) {
        if (this.IsEmerged(enemy)) return;
        enemy.StartEmergeAnimation();
    }

    public string GetSecondarySkillName(SandWormAI _) => "Emerge";

    public bool CanUseEntranceDoors(SandWormAI _) => false;

    public float InteractRange(SandWormAI _) => 0.0f;

    public bool SyncAnimationSpeedEnabled(SandWormAI _) => false;

}
