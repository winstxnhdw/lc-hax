using Hax;
using UnityEngine;

enum DocileLocustBeesState {
    ROAMING,
    HIDING
}

internal class DocileLocustBeesController : IEnemyController<DocileLocustBeesAI> {

    Vector3 camOffset = new(0, 2f, -4.5f);

    Vector3 enemyPositionOffset = new(0, 1.5f, 0);

    public Vector3 GetCameraOffset(DocileLocustBeesAI enemy) => this.camOffset;

    public Vector3 GetEnemyPositionOffset(DocileLocustBeesAI enemy) => this.enemyPositionOffset;

    bool isRoaming = true;

    public void OnPossess(DocileLocustBeesAI enemy) => this.isRoaming = true;

    public void UseSecondarySkill(DocileLocustBeesAI enemy) => this.isRoaming = false;

    public void ReleaseSecondarySkill(DocileLocustBeesAI enemy) => this.isRoaming = true;

    public void Update(DocileLocustBeesAI enemy, bool isAIControlled) {
        if (isAIControlled) return;
        if (this.isRoaming) {
            enemy.SetBehaviourState(DocileLocustBeesState.ROAMING);
        }
        else {
            enemy.SetBehaviourState(DocileLocustBeesState.HIDING);
        }
    }

    public bool SyncAnimationSpeedEnabled(EnemyAI enemy) => false;
}

