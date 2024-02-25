class TestEnemyController : IEnemyController<TestEnemy> {
    public bool CanUseEntranceDoors(TestEnemy _) => true;

    public float InteractRange(TestEnemy _) => 4.5f;
}

