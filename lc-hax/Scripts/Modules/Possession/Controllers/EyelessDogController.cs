using Hax;
using UnityEngine;

enum DogState {
    ROAMING,
    SUSPICIOUS,
    CHASE,
    LUNGE
}

class EyelessDogController : IEnemyController<MouthDogAI> {
    public Vector3 GetCameraOffset(MouthDogAI enemy) => new(0.0f, 3.2f, -4.0f);

    public void UsePrimarySkill(MouthDogAI enemy) => enemy.SetBehaviourState(enemy.IsBehaviourState(DogState.CHASE) ? DogState.ROAMING : DogState.CHASE);

    public void UseSecondarySkill(MouthDogAI enemy) => enemy.SetBehaviourState(DogState.LUNGE);

    public string GetSecondarySkillName(MouthDogAI _) => "Lunge";

    public void OnOutsideStatusChange(MouthDogAI enemy) => enemy.StopSearch(enemy.roamPlanet, true);
}
