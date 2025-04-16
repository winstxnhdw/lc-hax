class EarthLeviathanController : IEnemyController<SandWormAI> {
    static bool IsEmerged(SandWormAI enemy) => enemy.inEmergingState || enemy.emerged;

    public void UseSecondarySkill(SandWormAI enemy) {
        if (EarthLeviathanController.IsEmerged(enemy)) return;
        enemy.StartEmergeAnimation();
    }

    public string GetSecondarySkillName(SandWormAI _) => "Emerge";

    public bool CanUseEntranceDoors(SandWormAI _) => false;

    public float InteractRange(SandWormAI _) => 0.0f;

    public bool SyncAnimationSpeedEnabled(SandWormAI _) => false;

}
