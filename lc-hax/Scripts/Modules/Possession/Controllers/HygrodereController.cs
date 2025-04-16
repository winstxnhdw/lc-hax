class HygrodereController : IEnemyController<BlobAI> {
    static void SetTamedTimer(BlobAI enemy, float time) => enemy.Reflect().SetInternalField("tamedTimer", time);

    static void SetAngeredTimer(BlobAI enemy, float time) => enemy.Reflect().SetInternalField("angeredTimer", time);

    public void OnSecondarySkillHold(BlobAI enemy) {
        HygrodereController.SetAngeredTimer(enemy, 0.0f);
        HygrodereController.SetTamedTimer(enemy, 2.0f);
    }

    public void ReleaseSecondarySkill(BlobAI enemy) => HygrodereController.SetTamedTimer(enemy, 0.0f);

    public void UsePrimarySkill(BlobAI enemy) => HygrodereController.SetAngeredTimer(enemy, 18.0f);

    public float InteractRange(BlobAI _) => 3.5f;

    public float SprintMultiplier(BlobAI _) => 9.8f;
}

