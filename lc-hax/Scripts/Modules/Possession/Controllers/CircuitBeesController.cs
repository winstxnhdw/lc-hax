using Hax;
using UnityEngine;

enum BeesState {
    IDLE,
    DEFENSIVE,
    ATTACK
}

class CircuitBeesController : IEnemyController<RedLocustBees> {
    Vector3 CameraOffset { get; } = new(0.0f, 2.0f, -3.0f);

    public Vector3 GetCameraOffset(RedLocustBees _) => this.CameraOffset;

    public bool CanUseEntranceDoors(RedLocustBees _) => true;

    public void UsePrimarySkill(RedLocustBees enemy) {
        enemy.SetBehaviourState(BeesState.ATTACK);
        enemy.EnterAttackZapModeServerRpc(-1);
    }

    public void UseSecondarySkill(RedLocustBees enemy) => enemy.SetBehaviourState(BeesState.IDLE);

    public void OnOutsideStatusChange(RedLocustBees enemy) => enemy.StopSearch(enemy.searchForHive, true);
}

