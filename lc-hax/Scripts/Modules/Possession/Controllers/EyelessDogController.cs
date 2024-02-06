internal class EyelessDogController : IEnemyController<MouthDogAI> {
    internal void UseSecondarySkill(MouthDogAI enemyInstance) {
        if (enemyInstance.currentBehaviourStateIndex is 2) return;
        enemyInstance.SwitchToBehaviourServerRpc(2);
    }

    internal string GetSecondarySkillName(MouthDogAI _) => "Lunge";
}
