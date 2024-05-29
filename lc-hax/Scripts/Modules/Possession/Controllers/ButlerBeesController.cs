using UnityEngine;

internal class ButlerBeesController : IEnemyController<ButlerBeesEnemyAI>
{
    private Vector3 CamOffset { get; } = new(0, 2f, -3f);

    public Vector3 GetCameraOffset(ButlerBeesEnemyAI _)
    {
        return CamOffset;
    }
}