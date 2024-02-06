using Hax;

enum MouthDog {
    Roaming = 0,
    Suspicious = 1,
    ChaseMode = 2,
    Lunge = 3
}

internal class EyelessDogController : IEnemyController<MouthDogAI> {

    internal void OnMovement(MouthDogAI enemyInstance, bool isMoving, bool isSprinting) {
        if (!isSprinting) {
            if (isMoving) {
                enemyInstance.SetBehaviourState(MouthDog.Roaming);
            }
        }
        else {
            enemyInstance.SetBehaviourState(MouthDog.ChaseMode);
        }
    }

    internal void UseSecondarySkill(MouthDogAI enemyInstance) => enemyInstance.SetBehaviourState(MouthDog.Lunge);

    internal string GetSecondarySkillName(MouthDogAI _) => "Lunge";
}
