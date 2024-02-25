using Hax;

enum DogState {
    ROAMING,
    SUSPICIOUS,
    CHASE,
    LUNGE
}

class EyelessDogController : IEnemyController<MouthDogAI> {
    public void OnMovement(MouthDogAI enemy, bool isMoving, bool isSprinting) {
        if (!isSprinting) {
            if (!isMoving) return;
            enemy.SetBehaviourState(DogState.ROAMING);
        }

        else {
            enemy.SetBehaviourState(DogState.CHASE);
        }
    }

    public void UseSecondarySkill(MouthDogAI enemy) => enemy.SetBehaviourState(DogState.LUNGE);

    public string GetSecondarySkillName(MouthDogAI _) => "Lunge";

    public float InteractRange(MouthDogAI _) => 2.5f;
}
