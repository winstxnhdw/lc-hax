using Hax;
using UnityEngine;

enum DocileLocustBeesState {
    ROAMING,
    HIDING
}

internal class DocileLocustBeesController : IEnemyController<DocileLocustBeesAI> {

    Vector3 CamOffset = new(0, 2f, -4.5f);

    Vector3 EnemyPositionOffset = new(0, 1.5f, 0);

    public Vector3 GetCameraOffset(DocileLocustBeesAI enemy) => this.CamOffset;

    public Vector3 GetEnemyPositionOffset(DocileLocustBeesAI enemy) => this.EnemyPositionOffset;

    bool IsRoaming = true;

    public void OnPossess(DocileLocustBeesAI enemy) => this.IsRoaming = true;

    public void UseSecondarySkill(DocileLocustBeesAI enemy) => this.IsRoaming = false;

    public void ReleaseSecondarySkill(DocileLocustBeesAI enemy) => this.IsRoaming = true;

    public void Update(DocileLocustBeesAI enemy, bool isAIControlled) {
        if (isAIControlled) return;
        if (this.IsRoaming) {
            enemy.SetBehaviourState(DocileLocustBeesState.ROAMING);
        }
        else {
            enemy.SetBehaviourState(DocileLocustBeesState.HIDING);
        }
    }

    public bool SyncAnimationSpeedEnabled(EnemyAI enemy) => false;
}

