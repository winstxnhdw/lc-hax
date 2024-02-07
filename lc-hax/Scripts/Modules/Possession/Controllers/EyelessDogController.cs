using Hax;

enum MouthDog {
    ROAMING = 0,
    SUSPICIOUS = 1,
    CHASE = 2,
    LUNGE = 3
}

internal class EyelessDogController : IEnemyController<MouthDogAI> {

    private bool isSecondarySkillActive = false;

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
        if(!this.isSecondarySkillActive) {
            enemyInstance.SetBehaviourState(MouthDog.LUNGE);
            this.isSecondarySkillActive = true;
        }
    }

    public void ReleaseSecondarySkill(MouthDogAI enemyInstance) {
        if (this.isSecondarySkillActive) {
            this.isSecondarySkillActive = false;
        }
    }

    public string GetSecondarySkillName(MouthDogAI _) => "Lunge";
}
