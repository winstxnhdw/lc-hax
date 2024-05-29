using Hax;
using UnityEngine;

internal enum BeesState
{
    IDLE,
    DEFENSIVE,
    ATTACK
}

internal class CircuitBeesController : IEnemyController<RedLocustBees>
{
    private Vector3 CamOffset { get; } = new(0, 2f, -3f);

    public Vector3 GetCameraOffset(RedLocustBees _)
    {
        return CamOffset;
    }

    public void UsePrimarySkill(RedLocustBees enemy)
    {
        enemy.SetBehaviourState(BeesState.ATTACK);
        enemy.EnterAttackZapModeServerRpc(-1);
    }

    public void UseSecondarySkill(RedLocustBees enemy)
    {
        enemy.SetBehaviourState(BeesState.IDLE);
    }

    public void OnOutsideStatusChange(RedLocustBees enemy)
    {
        enemy.StopSearch(enemy.searchForHive, true);
    }
}