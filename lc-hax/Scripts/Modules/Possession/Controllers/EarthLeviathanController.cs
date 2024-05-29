using UnityEngine;

internal class EarthLeviathanController : IEnemyController<SandWormAI>
{
    private Vector3 CamOffset { get; } = new(0, 8f, -13f);

    public Vector3 GetCameraOffset(SandWormAI _)
    {
        return CamOffset;
    }

    public void UseSecondarySkill(SandWormAI enemy)
    {
        if (IsEmerged(enemy)) return;
        enemy.StartEmergeAnimation();
    }

    public bool IsAbleToMove(SandWormAI enemy)
    {
        return !enemy.inSpecialAnimation;
    }

    public bool IsAbleToRotate(SandWormAI enemy)
    {
        return !enemy.inSpecialAnimation;
    }

    public string GetSecondarySkillName(SandWormAI _)
    {
        return "Emerge";
    }

    public bool CanUseEntranceDoors(SandWormAI _)
    {
        return false;
    }

    public float InteractRange(SandWormAI _)
    {
        return 0.0f;
    }

    public bool SyncAnimationSpeedEnabled(SandWormAI _)
    {
        return false;
    }

    public void OnOutsideStatusChange(SandWormAI enemy)
    {
        enemy.StopSearch(enemy.roamMap, true);
    }

    private bool IsEmerged(SandWormAI enemy)
    {
        return enemy.inEmergingState || enemy.emerged;
    }
}