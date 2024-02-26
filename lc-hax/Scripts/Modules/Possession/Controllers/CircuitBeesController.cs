using Hax;
using UnityEngine;


enum BeesState {
    IDLE,
    DEFENSIVE,
    ATTACK
}

internal class CircuitBeesController : IEnemyController<RedLocustBees> {

    Vector3 CamOffset { get; } = new(0, 2f, -3f);

    public Vector3 GetCameraOffset(RedLocustBees _) => this.CamOffset;
    public bool CanUseEntranceDoors(RedLocustBees _) => true;

    public float InteractRange(RedLocustBees _) => 2.5f;

    public void UsePrimarySkill(RedLocustBees enemy) {
        enemy.SetBehaviourState(BeesState.ATTACK);
        enemy.EnterAttackZapModeServerRpc(-1);
    }

    public void UseSecondarySkill(RedLocustBees enemy) => enemy.SetBehaviourState(BeesState.IDLE);

    public void OnOutsideStatusChange(RedLocustBees enemy) => enemy.StopSearch(enemy.searchForHive, true);

}

