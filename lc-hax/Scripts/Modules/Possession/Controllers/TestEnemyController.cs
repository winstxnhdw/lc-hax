internal class TestEnemyController : IEnemyController<TestEnemy> {
    public void GetCameraPosition(TestEnemy enemy) {
        PossessionMod.CamOffsetY = 2f;
        PossessionMod.CamOffsetZ = -4.5f;
        PossessionMod.EnemyYPositionOffset = 1.5f;
    }

    public bool CanUseEntranceDoors(TestEnemy _) => true;

    public float InteractRange(TestEnemy _) => 4.5f;
}

