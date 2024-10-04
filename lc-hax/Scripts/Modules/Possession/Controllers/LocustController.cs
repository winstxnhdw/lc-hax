using Hax;
using UnityEngine;

enum LocustState {
    ROAMING,
    HIDING
}

class LocustController : IEnemyController<DocileLocustBeesAI> {
    Vector3 enemyPositionOffset = new(0.0f, 1.5f, 0.0f);

    bool IsRoaming { get; set; } = true;

    public Vector3 GetCameraOffset(DocileLocustBeesAI enemy) => new(0.0f, 2.0f, -4.5f);

    public Vector3 GetEnemyPositionOffset(DocileLocustBeesAI enemy) => this.enemyPositionOffset;

    public void OnPossess(DocileLocustBeesAI enemy) => this.IsRoaming = true;

    public void UseSecondarySkill(DocileLocustBeesAI enemy) => this.IsRoaming = false;

    public void ReleaseSecondarySkill(DocileLocustBeesAI enemy) => this.IsRoaming = true;

    public bool SyncAnimationSpeedEnabled(DocileLocustBeesAI enemy) => false;

    public void Update(DocileLocustBeesAI enemy, bool isAIControlled) {
        if (isAIControlled) return;
        if (this.IsRoaming) {
            enemy.SetBehaviourState(LocustState.ROAMING);
        }

        else {
            enemy.SetBehaviourState(LocustState.HIDING);
        }
    }
}

