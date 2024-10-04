using UnityEngine;

class TestEnemyController : IEnemyController<TestEnemy> {
    public Vector3 GetCameraOffset(TestEnemy enemy) => new(0.0f, 2.0f, -4.5f);

    public Vector3 GetEnemyPositionOffset(TestEnemy enemy) => new(0.0f, 1.5f, 0.0f);

    public bool CanUseEntranceDoors(TestEnemy _) => true;
}

