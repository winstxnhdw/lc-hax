using Hax;
using UnityEngine;

enum DogState {
    ROAMING,
    SUSPICIOUS,
    CHASE,
    LUNGE
}

internal class EyelessDogController : IEnemyController<MouthDogAI> {

    Vector3 CamOffset = new Vector3(0, 3.2f, -4f);

    public Vector3 GetCameraOffset(MouthDogAI enemy) => this.CamOffset;

    public void UsePrimarySkill(MouthDogAI enemy) => enemy.SetBehaviourState(enemy.IsBehaviourState(DogState.CHASE) ? DogState.ROAMING : DogState.CHASE);



    public void UseSecondarySkill(MouthDogAI enemy) => enemy.SetBehaviourState(DogState.LUNGE);

    public string GetSecondarySkillName(MouthDogAI _) => "Lunge";

    public float InteractRange(MouthDogAI _) => 2.5f;

    public void OnOutsideStatusChange(MouthDogAI enemy) => enemy.StopSearch(enemy.roamPlanet, true);



}
