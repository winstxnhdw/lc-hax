using Hax;
using UnityEngine;

internal enum GiantState
{
    DEFAULT = 0,
    CHASE = 1
}

internal class ForestGiantController : IEnemyController<ForestGiantAI>
{
    private readonly Vector3 camOffset = new(0, 8f, -8f);

    private bool IsUsingSecondarySkill { get; set; } = false;

    public Vector3 GetCameraOffset(ForestGiantAI enemy)
    {
        return camOffset;
    }

    public void Update(ForestGiantAI enemy, bool isAIControlled)
    {
        if (!IsUsingSecondarySkill)
            enemy.SetBehaviourState(GiantState.DEFAULT);
        else
            enemy.SetBehaviourState(GiantState.CHASE);
    }


    public void OnSecondarySkillHold(ForestGiantAI enemy)
    {
        IsUsingSecondarySkill = true;
        enemy.SetBehaviourState(GiantState.CHASE);
    }

    public void ReleaseSecondarySkill(ForestGiantAI enemy)
    {
        IsUsingSecondarySkill = false;
        enemy.SetBehaviourState(GiantState.DEFAULT);
    }

    public bool IsAbleToMove(ForestGiantAI enemy)
    {
        return !enemy.Reflect().GetInternalField<bool>("inEatingPlayerAnimation");
    }

    public string GetSecondarySkillName(ForestGiantAI _)
    {
        return "(HOLD) Chase";
    }

    public bool CanUseEntranceDoors(ForestGiantAI _)
    {
        return false;
    }

    public float InteractRange(ForestGiantAI _)
    {
        return 0f;
    }

    public void OnUnpossess(ForestGiantAI enemy)
    {
        IsUsingSecondarySkill = false;
    }

    public bool SyncAnimationSpeedEnabled(ForestGiantAI _)
    {
        return false;
    }

    public void OnOutsideStatusChange(ForestGiantAI enemy)
    {
        enemy.StopSearch(enemy.roamPlanet, true);
        enemy.StopSearch(enemy.searchForPlayers, true);
    }
}