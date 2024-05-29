using Hax;
using UnityEngine;

internal enum DogState
{
    ROAMING,
    SUSPICIOUS,
    CHASE,
    LUNGE
}

internal class EyelessDogController : IEnemyController<MouthDogAI>
{
    private readonly Vector3 camOffset = new(0, 3.2f, -4f);

    public Vector3 GetCameraOffset(MouthDogAI enemy)
    {
        return camOffset;
    }

    public void UsePrimarySkill(MouthDogAI enemy)
    {
        enemy.SetBehaviourState(enemy.IsBehaviourState(DogState.CHASE) ? DogState.ROAMING : DogState.CHASE);
    }

    public void UseSecondarySkill(MouthDogAI enemy)
    {
        enemy.SetBehaviourState(DogState.LUNGE);
    }

    public string GetSecondarySkillName(MouthDogAI _)
    {
        return "Lunge";
    }

    public void OnOutsideStatusChange(MouthDogAI enemy)
    {
        enemy.StopSearch(enemy.roamPlanet, true);
    }
}