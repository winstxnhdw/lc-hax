using UnityEngine;

internal class LassoManController : IEnemyController<LassoManAI>
{
    private readonly Vector3 CamOffset = new(0, 2.8f, -3.5f);

    public Vector3 GetCameraOffset(LassoManAI enemy)
    {
        return CamOffset;
    }

    public void UsePrimarySkill(LassoManAI enemy)
    {
        enemy.MakeScreechNoiseServerRpc();
    }

    public bool SyncAnimationSpeedEnabled(LassoManAI enemy)
    {
        return false;
    }

    public void OnOutsideStatusChange(LassoManAI enemy)
    {
        enemy.StopSearch(enemy.searchForPlayers, true);
    }
}