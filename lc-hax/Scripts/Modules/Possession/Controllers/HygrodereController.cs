using UnityEngine;

internal class HygrodereController : IEnemyController<BlobAI>
{
    private readonly Vector3 camOffset = new(0, 2f, -3f);

    public Vector3 GetCameraOffset(BlobAI enemy)
    {
        return camOffset;
    }

    public void OnSecondarySkillHold(BlobAI enemy)
    {
        SetAngeredTimer(enemy, 0.0f);
        SetTamedTimer(enemy, 2.0f);
    }

    public void ReleaseSecondarySkill(BlobAI enemy)
    {
        SetTamedTimer(enemy, 0.0f);
    }

    public void UsePrimarySkill(BlobAI enemy)
    {
        SetAngeredTimer(enemy, 18.0f);
    }

    public float SprintMultiplier(BlobAI _)
    {
        return 9.8f;
    }

    public void OnOutsideStatusChange(BlobAI enemy)
    {
        enemy.StopSearch(enemy.searchForPlayers, true);
    }


    private void SetTamedTimer(BlobAI enemy, float time)
    {
        enemy.Reflect().SetInternalField("tamedTimer", time);
    }

    private void SetAngeredTimer(BlobAI enemy, float time)
    {
        enemy.Reflect().SetInternalField("angeredTimer", time);
    }
}