using Hax;

enum MouthDog {
    ROAMING,
    SUSPICIOUS,
    CHASE,
    LUNGE
}

internal class EyelessDogController : IEnemyController<MouthDogAI> {

    public void OnMovement(MouthDogAI enemy, bool isMoving, bool isSprinting) {
        if (!isSprinting) {
            if (!isMoving) return;
            enemy.SetBehaviourState(MouthDog.ROAMING);
        }

        else {
            enemy.SetBehaviourState(MouthDog.CHASE);
        }
    }

    public void UseSecondarySkill(MouthDogAI enemy) => enemy.SetBehaviourState(MouthDog.LUNGE);

    public string GetSecondarySkillName(MouthDogAI _) => "Lunge";

    public float InteractRange(MouthDogAI _) => 2.5f;

}
