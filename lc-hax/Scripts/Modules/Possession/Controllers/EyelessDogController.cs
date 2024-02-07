using Hax;

enum MouthDog {
    ROAMING = 0,
    SUSPICIOUS = 1,
    CHASE = 2,
    LUNGE = 3
}

internal class EyelessDogController : IEnemyController<MouthDogAI> {
    bool IsSecondarySkillActive { get; set; } = false;

    public void OnMovement(MouthDogAI enemyInstance, bool isMoving, bool isSprinting) {
        if (!isSprinting) {
            if (!isMoving) return;
            enemyInstance.SetBehaviourState(MouthDog.ROAMING);
        }

        else {
            enemyInstance.SetBehaviourState(MouthDog.CHASE);
        }
    }

    public void UseSecondarySkill(MouthDogAI enemyInstance) {
        if (this.IsSecondarySkillActive) return;

        this.IsSecondarySkillActive = true;
        enemyInstance.SetBehaviourState(MouthDog.LUNGE);
    }

    public void ReleaseSecondarySkill(MouthDogAI enemyInstance) {
        if (!this.IsSecondarySkillActive) return;

        this.IsSecondarySkillActive = false;
        enemyInstance.SetBehaviourState(MouthDog.CHASE);
    }

    public string GetSecondarySkillName(MouthDogAI _) => "Lunge";
}
