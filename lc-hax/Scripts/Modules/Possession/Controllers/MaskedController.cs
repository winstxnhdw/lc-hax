using UnityEngine;

internal class MaskedPlayerController : IEnemyController<MaskedPlayerEnemy>
{
    private readonly Vector3 camOffset = new(0, 2.8f, -3f);

    public Vector3 GetCameraOffset(MaskedPlayerEnemy enemy)
    {
        return camOffset;
    }

    public void UsePrimarySkill(MaskedPlayerEnemy enemy)
    {
        enemy.SetHandsOutServerRpc(!enemy.Reflect().GetInternalField<bool>("HandsOut"));
    }

    public void UseSecondarySkill(MaskedPlayerEnemy enemy)
    {
        enemy.SetCrouchingServerRpc(!enemy.Reflect().GetInternalField<bool>("crouching"));
    }

    public bool IsAbleToMove(MaskedPlayerEnemy enemy)
    {
        return !enemy.Reflect().GetInternalField<bool>("inKillAnimation");
    }

    public bool IsAbleToRotate(MaskedPlayerEnemy enemy)
    {
        return !enemy.Reflect().GetInternalField<bool>("inKillAnimation");
    }

    public bool SyncAnimationSpeedEnabled(MaskedPlayerEnemy _)
    {
        return false;
    }

    public void OnOutsideStatusChange(MaskedPlayerEnemy enemy)
    {
        enemy.StopSearch(enemy.searchForPlayers, true);
    }
}