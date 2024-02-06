using Hax;

enum MouthDog {
    ROAMING = 0,
    SUSPICIOUS = 1,
    CHASE = 2,
    LUNGE = 3
}

internal class EyelessDogController : IEnemyController<MouthDogAI> {

    public void OnMovement(MouthDogAI enemyInstance, bool isMoving, bool isSprinting) {
        if (!isSprinting) {
            if (!isMoving) return;
            enemyInstance.SetBehaviourState(MouthDog.ROAMING);
        }

        else {
            enemyInstance.SetBehaviourState(MouthDog.CHASE);
        }
    }

    public void UseSecondarySkill(MouthDogAI enemyInstance) => enemyInstance.SetBehaviourState(MouthDog.LUNGE);

    public CharArray GetSecondarySkillName(MouthDogAI _) => "Lunge";
}
