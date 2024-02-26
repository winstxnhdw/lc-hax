using UnityEngine;

internal class TestEnemyController : IEnemyController<TestEnemy> {

    Vector3 CamOffset = new Vector3(0, 2f, -4.5f);

    Vector3 EnemyPositionOffset = new Vector3(0, 1.5f, 0);

    public Vector3 GetCameraOffset(TestEnemy enemy) => this.CamOffset;

    public Vector3 GetEnemyPositionOffset(TestEnemy enemy) => this.EnemyPositionOffset;

  

    public bool CanUseEntranceDoors(TestEnemy _) => true;

    public float InteractRange(TestEnemy _) => 4.5f;
}

