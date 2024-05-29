using Hax;
using UnityEngine;

internal enum DocileLocustBeesState
{
    ROAMING,
    HIDING
}

internal class DocileLocustBeesController : IEnemyController<DocileLocustBeesAI>
{
    private readonly Vector3 camOffset = new(0, 2f, -4.5f);

    private readonly Vector3 enemyPositionOffset = new(0, 1.5f, 0);

    private bool isRoaming = true;

    public Vector3 GetCameraOffset(DocileLocustBeesAI enemy)
    {
        return camOffset;
    }

    public Vector3 GetEnemyPositionOffset(DocileLocustBeesAI enemy)
    {
        return enemyPositionOffset;
    }

    public void OnPossess(DocileLocustBeesAI enemy)
    {
        isRoaming = true;
    }

    public void UseSecondarySkill(DocileLocustBeesAI enemy)
    {
        isRoaming = false;
    }

    public void ReleaseSecondarySkill(DocileLocustBeesAI enemy)
    {
        isRoaming = true;
    }

    public void Update(DocileLocustBeesAI enemy, bool isAIControlled)
    {
        if (isAIControlled) return;
        if (isRoaming)
            enemy.SetBehaviourState(DocileLocustBeesState.ROAMING);
        else
            enemy.SetBehaviourState(DocileLocustBeesState.HIDING);
    }

    public bool SyncAnimationSpeedEnabled(DocileLocustBeesAI enemy)
    {
        return false;
    }
}