using Hax;
using UnityEngine;

internal enum DoublewingBirdState
{
    IDLE,
    GLIDING
}

internal class DoublewingBirdController : IEnemyController<DoublewingAI>
{
    public Vector3 camOffsets = new(0, 2.3f, -3f);
    public float transitionSpeed = 0f;

    public Vector3 GetCameraOffset(DoublewingAI enemy)
    {
        float targetCamOffsetY, targetCamOffsetZ;

        if (enemy.IsBehaviourState(DoublewingBirdState.GLIDING))
        {
            // In Air
            transitionSpeed = 1.2f;
            targetCamOffsetY = 22.5f;
            targetCamOffsetZ = -3.5f;
        }
        else
        {
            // On Ground
            transitionSpeed = 1.3f;
            targetCamOffsetY = 2.3f;
            targetCamOffsetZ = -3f;
        }

        camOffsets.y = Mathf.Lerp(camOffsets.y, targetCamOffsetY, Time.deltaTime * transitionSpeed);
        camOffsets.z = Mathf.Lerp(camOffsets.z, targetCamOffsetZ, Time.deltaTime * transitionSpeed);

        return camOffsets;
    }

    public void UsePrimarySkill(DoublewingAI enemy)
    {
        enemy.SetBehaviourState(DoublewingBirdState.IDLE);
    }

    public void UseSecondarySkill(DoublewingAI enemy)
    {
        enemy.SetBehaviourState(DoublewingBirdState.GLIDING);
    }

    public bool IsAbleToRotate(DoublewingAI enemy)
    {
        return !enemy.IsBehaviourState(DoublewingBirdState.IDLE);
    }

    public bool SyncAnimationSpeedEnabled(DoublewingAI enemy)
    {
        return true;
    }
}