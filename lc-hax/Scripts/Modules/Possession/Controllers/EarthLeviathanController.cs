using UnityEngine;

internal class EarthLeviathanController : IEnemyController<SandWormAI> {

    Vector3 CamOffset { get; } = new(0, 8f, -13f);

    public Vector3 GetCameraOffset(SandWormAI _) => this.CamOffset;

    bool IsEmerged(SandWormAI enemy) => enemy.inEmergingState || enemy.emerged;

    public void UseSecondarySkill(SandWormAI enemy) {
        if (this.IsEmerged(enemy)) return;
        enemy.StartEmergeAnimation();
    }

    public bool IsAbleToMove(SandWormAI enemy) => !enemy.inSpecialAnimation;

    public bool IsAbleToRotate(SandWormAI enemy) => !enemy.inSpecialAnimation;

    public string GetSecondarySkillName(SandWormAI _) => "Emerge";

    public bool CanUseEntranceDoors(SandWormAI _) => false;

    public float InteractRange(SandWormAI _) => 0.0f;

    public bool SyncAnimationSpeedEnabled(SandWormAI _) => false;
    public void OnOutsideStatusChange(SandWormAI enemy) => enemy.StopSearch(enemy.roamMap, true);

}
