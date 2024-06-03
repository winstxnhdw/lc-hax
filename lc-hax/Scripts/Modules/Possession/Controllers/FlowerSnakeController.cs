#region

using UnityEngine;

#endregion

class FlowerSnakeController : IEnemyController<FlowerSnakeEnemy> {
    public Vector3 CamOffsets = new(0, 2.5f, -3f);
    public float transitionSpeed = 0f;

    public Vector3 GetCameraOffset(FlowerSnakeEnemy enemy) {
        float targetCamOffsetY, targetCamOffsetZ;

        if (enemy.clingingToPlayer is null) {
            // Is Roaming
            this.transitionSpeed = 8.0f;
            targetCamOffsetY = 2f;
            targetCamOffsetZ = -4f;
        }
        else {
            // On Player
            this.transitionSpeed = 4.5f;
            targetCamOffsetY = 0f;
            targetCamOffsetZ = -2f;
        }

        // Smoothly interpolate between current and target camera positions
        this.CamOffsets.y = Mathf.Lerp(this.CamOffsets.y, targetCamOffsetY, Time.deltaTime * this.transitionSpeed);
        this.CamOffsets.z = Mathf.Lerp(this.CamOffsets.z, targetCamOffsetZ, Time.deltaTime * this.transitionSpeed);

        return this.CamOffsets;
    }

    public string GetPrimarySkillName(FlowerSnakeEnemy _) => "";

    public string GetSecondarySkillName(FlowerSnakeEnemy _) => "";

    public bool SyncAnimationSpeedEnabled(FlowerSnakeEnemy _) => true;

    bool IsClingingToSomething(FlowerSnakeEnemy enemy) =>
        enemy.clingingToPlayer is not null || enemy.inSpecialAnimation;
}
