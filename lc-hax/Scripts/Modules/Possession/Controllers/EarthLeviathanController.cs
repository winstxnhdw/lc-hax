using UnityEngine;

class EarthLeviathanController : IEnemyController<SandWormAI> {
    public Vector3 GetCameraOffset(SandWormAI _) => new(0.0f, 8.0f, -13.0f);

    public void UseSecondarySkill(SandWormAI enemy) {
        if (enemy.inEmergingState || enemy.emerged) return;
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
