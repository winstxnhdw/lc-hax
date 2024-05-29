using UnityEngine;

internal class FlowerSnakeController : IEnemyController<FlowerSnakeEnemy>
{
    public Vector3 CamOffsets = new(0, 2.5f, -3f);
    public float transitionSpeed = 0f;

    public Vector3 GetCameraOffset(FlowerSnakeEnemy enemy)
    {
        float targetCamOffsetY, targetCamOffsetZ;

        if (enemy.clingingToPlayer is null)
        {
            // Is Roaming
            transitionSpeed = 8.0f;
            targetCamOffsetY = 2f;
            targetCamOffsetZ = -4f;
        }
        else
        {
            // On Player
            transitionSpeed = 4.5f;
            targetCamOffsetY = 0f;
            targetCamOffsetZ = -2f;
        }

        // Smoothly interpolate between current and target camera positions
        CamOffsets.y = Mathf.Lerp(CamOffsets.y, targetCamOffsetY, Time.deltaTime * transitionSpeed);
        CamOffsets.z = Mathf.Lerp(CamOffsets.z, targetCamOffsetZ, Time.deltaTime * transitionSpeed);

        return CamOffsets;
    }

    public string GetPrimarySkillName(FlowerSnakeEnemy _)
    {
        return "";
    }

    public string GetSecondarySkillName(FlowerSnakeEnemy _)
    {
        return "";
    }

    public bool SyncAnimationSpeedEnabled(FlowerSnakeEnemy _)
    {
        return true;
    }

    private bool IsClingingToSomething(FlowerSnakeEnemy enemy)
    {
        return enemy.clingingToPlayer is not null || enemy.inSpecialAnimation;
    }
}