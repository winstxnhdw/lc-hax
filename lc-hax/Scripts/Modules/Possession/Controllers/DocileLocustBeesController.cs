using Hax;
using UnityEngine;

enum DocileLocustBeesState {
    ROAMING,
    HIDING
}

internal class DocileLocustBeesController : IEnemyController<DocileLocustBeesAI> {

    Vector3 CamOffset = new Vector3(0, 2f, -4.5f);

    Vector3 EnemyPositionOffset = new Vector3(0, 1.5f, 0);

    public Vector3 GetCameraOffset(DocileLocustBeesAI enemy) => this.CamOffset;

    public Vector3 GetEnemyPositionOffset(DocileLocustBeesAI enemy) => this.EnemyPositionOffset;

    bool IsRoaming = true;

    public void OnPossess(DocileLocustBeesAI enemy) => IsRoaming = true;

    public void UseSecondarySkill(DocileLocustBeesAI enemy) => IsRoaming = false;

    public void ReleaseSecondarySkill(DocileLocustBeesAI enemy) => IsRoaming = true;

    public void Update(DocileLocustBeesAI enemy, bool isAIControlled) {
        if (isAIControlled) return;
        if (IsRoaming) {
            enemy.SetBehaviourState(DocileLocustBeesState.ROAMING);
        }
        else {
            enemy.SetBehaviourState(DocileLocustBeesState.HIDING);
        }
    }

    public bool SyncAnimationSpeedEnabled(EnemyAI enemy) => false;
}

