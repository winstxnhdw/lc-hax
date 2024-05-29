using UnityEngine;

internal class TestEnemyController : IEnemyController<TestEnemy>
{
    private readonly Vector3 camOffset = new(0, 2f, -4.5f);

    private readonly Vector3 enemyPositionOffset = new(0, 1.5f, 0);

    public Vector3 GetCameraOffset(TestEnemy enemy)
    {
        return camOffset;
    }

    public Vector3 GetEnemyPositionOffset(TestEnemy enemy)
    {
        return enemyPositionOffset;
    }
}