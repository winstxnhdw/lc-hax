using Hax;
using UnityEngine;

internal enum SnareFleaState
{
    SEARCHING,
    HIDING,
    CHASING,
    CLINGING
}

internal class SnareFleaController : IEnemyController<CentipedeAI>
{
    public Vector3 CamOffsets = new(0, 2.5f, -3f);
    public float transitionSpeed = 0f;

    public Vector3 GetCameraOffset(CentipedeAI enemy)
    {
        float targetCamOffsetY, targetCamOffsetZ;

        if (!enemy.IsBehaviourState(SnareFleaState.HIDING) && enemy.clingingToPlayer is null)
        {
            // Is Roaming
            transitionSpeed = 8.0f;
            targetCamOffsetY = 2f;
            targetCamOffsetZ = -4f;
        }
        else if (enemy.clingingToPlayer is not null)
        {
            // On Player
            transitionSpeed = 4.5f;
            targetCamOffsetY = 0f;
            targetCamOffsetZ = -2f;
        }
        else
        {
            // On Ceiling
            transitionSpeed = 2.5f;
            targetCamOffsetY = -0.3f;
            targetCamOffsetZ = 0f;
        }

        // Smoothly interpolate between current and target camera positions
        CamOffsets.y = Mathf.Lerp(CamOffsets.y, targetCamOffsetY, Time.deltaTime * transitionSpeed);
        CamOffsets.z = Mathf.Lerp(CamOffsets.z, targetCamOffsetZ, Time.deltaTime * transitionSpeed);

        return CamOffsets;
    }

    public void UsePrimarySkill(CentipedeAI enemy)
    {
        if (!enemy.IsBehaviourState(SnareFleaState.HIDING)) return;
        enemy.SetBehaviourState(SnareFleaState.CHASING);
    }

    public void UseSecondarySkill(CentipedeAI enemy)
    {
        if (IsClingingToSomething(enemy)) return;
        _ = enemy.Reflect().InvokeInternalMethod("RaycastToCeiling");
        enemy.SetBehaviourState(SnareFleaState.HIDING);
    }

    public bool IsAbleToMove(CentipedeAI enemy)
    {
        return !IsClingingToSomething(enemy);
    }

    public bool IsAbleToRotate(CentipedeAI enemy)
    {
        return !IsClingingToSomething(enemy);
    }

    public string GetPrimarySkillName(CentipedeAI _)
    {
        return "Drop";
    }

    public string GetSecondarySkillName(CentipedeAI _)
    {
        return "Attach to ceiling";
    }

    public bool CanUseEntranceDoors(CentipedeAI _)
    {
        return false;
    }

    public bool SyncAnimationSpeedEnabled(CentipedeAI _)
    {
        return false;
    }

    private bool IsClingingToSomething(CentipedeAI enemy)
    {
        var centipedeReflector = enemy.Reflect();

        return enemy.clingingToPlayer is not null || enemy.inSpecialAnimation ||
               centipedeReflector.GetInternalField<bool>("clingingToDeadBody") ||
               centipedeReflector.GetInternalField<bool>("clingingToCeiling") ||
               centipedeReflector.GetInternalField<bool>("startedCeilingAnimationCoroutine") ||
               centipedeReflector.GetInternalField<bool>("inDroppingOffPlayerAnim");
    }
}