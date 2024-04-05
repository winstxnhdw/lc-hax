using UnityEngine;

internal class TestEnemyController : IEnemyController<TestEnemy> {

    Vector3 camOffset = new(0, 2f, -4.5f);

    Vector3 enemyPositionOffset = new(0, 1.5f, 0);

    public Vector3 GetCameraOffset(TestEnemy enemy) => this.camOffset;

    public Vector3 GetEnemyPositionOffset(TestEnemy enemy) => this.enemyPositionOffset;

}

