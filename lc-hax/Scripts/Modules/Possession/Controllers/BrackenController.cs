using Hax;
using UnityEngine;

internal enum BrackenState
{
    SCOUTING,
    STAND,
    ANGER
}

internal class BrackenController : IEnemyController<FlowermanAI>
{
    public Vector3 CamOffsets = new(0, 2f, -3f);
    public float transitionSpeed = 0f;

    public Vector3 GetCameraOffset(FlowermanAI enemy)
    {
        float targetCamOffsetY, targetCamOffsetZ;

        if (enemy.IsBehaviourState(BrackenState.SCOUTING))
        {
            transitionSpeed = 4.0f;
            targetCamOffsetY = 2f;
            targetCamOffsetZ = -3f;
        }
        else
        {
            transitionSpeed = 4.0f;
            targetCamOffsetY = 2.6f;
            targetCamOffsetZ = -3.2f;
        }

        CamOffsets.y = Mathf.Lerp(CamOffsets.y, targetCamOffsetY, Time.deltaTime * transitionSpeed);
        CamOffsets.z = Mathf.Lerp(CamOffsets.z, targetCamOffsetZ, Time.deltaTime * transitionSpeed);

        return CamOffsets;
    }

    public void UsePrimarySkill(FlowermanAI enemy)
    {
        if (!enemy.carryingPlayerBody) enemy.SetBehaviourState(BrackenState.ANGER);

        enemy.DropPlayerBodyServerRpc();
    }

    public void UseSecondarySkill(FlowermanAI enemy)
    {
        enemy.SetBehaviourState(BrackenState.STAND);
    }

    public void ReleaseSecondarySkill(FlowermanAI enemy)
    {
        enemy.SetBehaviourState(BrackenState.SCOUTING);
    }

    public bool IsAbleToMove(FlowermanAI enemy)
    {
        return !enemy.inSpecialAnimation;
    }

    public string GetPrimarySkillName(FlowermanAI enemy)
    {
        return enemy.carryingPlayerBody ? "Drop body" : "";
    }

    public string GetSecondarySkillName(FlowermanAI _)
    {
        return "Stand";
    }

    public bool SyncAnimationSpeedEnabled(FlowermanAI _)
    {
        return false;
    }
}