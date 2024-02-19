using Hax;

enum MouthDog {
    ROAMING,
    SUSPICIOUS,
    CHASE,
    LUNGE
}

internal class EyelessDogController : IEnemyController<MouthDogAI> {


    public void UsePrimarySkill(MouthDogAI enemy) => enemy.SetBehaviourState(MouthDog.CHASE);

    public void UseSecondarySkill(MouthDogAI enemy) => enemy.SetBehaviourState(MouthDog.LUNGE);


    public string GetSecondarySkillName(MouthDogAI _) => "Lunge";

    public float InteractRange(MouthDogAI _) => 2.5f;

}
