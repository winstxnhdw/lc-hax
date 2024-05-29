using Hax;
using UnityEngine;

internal enum JesterState
{
    CLOSED,
    CRANKING,
    OPEN
}

internal class JesterController : IEnemyController<JesterAI>
{
    public Vector3 camOffsets = new(0, 2.5f, -3f);
    public float transitionSpeed = 5f;

    public Vector3 GetCameraOffset(JesterAI enemy)
    {
        float targetCamOffsetY, targetCamOffsetZ;

        if (!enemy.IsBehaviourState(JesterState.OPEN))
        {
            targetCamOffsetY = 2f;
            targetCamOffsetZ = -3f;
        }
        else
        {
            targetCamOffsetY = 2.3f;
            targetCamOffsetZ = -3.5f;
        }

        // Smoothly interpolate between current and target camera positions
        camOffsets.y = Mathf.Lerp(camOffsets.y, targetCamOffsetY, Time.deltaTime * transitionSpeed);
        camOffsets.z = Mathf.Lerp(camOffsets.z, targetCamOffsetZ, Time.deltaTime * transitionSpeed);

        return camOffsets;
    }

    public void UsePrimarySkill(JesterAI enemy)
    {
        enemy.SetBehaviourState(JesterState.CLOSED);
        SetNoPlayerChasetimer(enemy, 0.0f);
        enemy.mainCollider.isTrigger = false;
    }

    public void OnSecondarySkillHold(JesterAI enemy)
    {
        if (!enemy.IsBehaviourState(JesterState.CLOSED)) return;
        enemy.SetBehaviourState(JesterState.CRANKING);
    }

    public void ReleaseSecondarySkill(JesterAI enemy)
    {
        if (!enemy.IsBehaviourState(JesterState.CRANKING)) return;
        enemy.SetBehaviourState(JesterState.OPEN);
        enemy.mainCollider.isTrigger = true;
    }

    public void Update(JesterAI enemy, bool isAIControlled)
    {
        SetNoPlayerChasetimer(enemy, 100.0f);
    }

    public void OnUnpossess(JesterAI enemy)
    {
        SetNoPlayerChasetimer(enemy, 25.0f);
        camOffsets = new Vector3(0, 2.5f, -3f);
    }

    public void OnPossess(JesterAI enemy)
    {
        camOffsets = new Vector3(0, 2.5f, -3f);
    }


    public bool IsAbleToMove(JesterAI enemy)
    {
        return !enemy.IsBehaviourState(JesterState.CRANKING);
    }

    public bool IsAbleToRotate(JesterAI enemy)
    {
        return !enemy.IsBehaviourState(JesterState.CRANKING);
    }

    public string GetPrimarySkillName(JesterAI _)
    {
        return "Close box";
    }

    public string GetSecondarySkillName(JesterAI _)
    {
        return "(HOLD) Begin cranking";
    }

    public void OnOutsideStatusChange(JesterAI enemy)
    {
        enemy.StopSearch(enemy.roamMap, true);
    }

    private void SetNoPlayerChasetimer(JesterAI enemy, float value)
    {
        enemy.Reflect().SetInternalField("noPlayersToChaseTimer", value);
    }
}