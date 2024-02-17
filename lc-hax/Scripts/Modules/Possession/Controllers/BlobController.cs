internal class BlobController : IEnemyController<BlobAI> {
    public void SetTamedTimer(BlobAI instance, float time) => _ = instance.Reflect().SetInternalField("tamedTimer", time);

    public void SetAngeredTimer(BlobAI instance, float time) => _ = instance.Reflect().SetInternalField("angeredTimer", time);

    public void UseSecondarySkill(BlobAI enemyInstance) {
        this.SetAngeredTimer(enemyInstance, 0.0f);
        this.SetTamedTimer(enemyInstance, 2.0f);
    }

    public void ReleaseSecondarySkill(BlobAI enemyInstance) => this.SetTamedTimer(enemyInstance, 0.0f);

    public void UsePrimarySkill(BlobAI enemyInstance) => this.SetAngeredTimer(enemyInstance, 18.0f);

    public float InteractRange(BlobAI _) => 3.5f;

    public float SprintMultiplier(BlobAI _) => 9.8f;
}

