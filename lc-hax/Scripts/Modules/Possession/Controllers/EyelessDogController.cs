using Hax;

enum MouthDog {
    ROAMING,
    SUSPICIOUS,
    CHASE,
    LUNGE
}

internal class EyelessDogController : IEnemyController<MouthDogAI> {
    bool IsSecondarySkillActive { get; set; } = false;

    public void OnMovement(MouthDogAI enemy, bool isMoving, bool isSprinting) {
        if (!isSprinting) {
            if (!isMoving) return;
            enemy.SetBehaviourState(MouthDog.ROAMING);
        }

        else {
            enemy.SetBehaviourState(MouthDog.CHASE);
        }
    }

    public void UseSecondarySkill(MouthDogAI enemy) {
        if (this.IsSecondarySkillActive) return;

        this.IsSecondarySkillActive = true;
        enemy.SetBehaviourState(MouthDog.LUNGE);
    }

    public void ReleaseSecondarySkill(MouthDogAI enemy) {
        if (!this.IsSecondarySkillActive) return;

        this.IsSecondarySkillActive = false;
        enemy.SetBehaviourState(MouthDog.CHASE);
    }

    public string GetSecondarySkillName(MouthDogAI _) => "Lunge";

    public float InteractRange(MouthDogAI _) => 2.5f;

}
