#region

using Hax;
using UnityEngine;

#endregion

enum DocileLocustBeesState {
    ROAMING,
    HIDING
}

class DocileLocustBeesController : IEnemyController<DocileLocustBeesAI> {
    readonly Vector3 camOffset = new(0, 2f, -4.5f);

    readonly Vector3 enemyPositionOffset = new(0, 1.5f, 0);

    bool isRoaming = true;

    public Vector3 GetCameraOffset(DocileLocustBeesAI enemy) => this.camOffset;

    public Vector3 GetEnemyPositionOffset(DocileLocustBeesAI enemy) => this.enemyPositionOffset;

    public void OnPossess(DocileLocustBeesAI enemy) => this.isRoaming = true;

    public void UseSecondarySkill(DocileLocustBeesAI enemy) => this.isRoaming = false;

    public void ReleaseSecondarySkill(DocileLocustBeesAI enemy) => this.isRoaming = true;

    public void Update(DocileLocustBeesAI enemy, bool isAIControlled) {
        if (isAIControlled) return;
        if (this.isRoaming)
            enemy.SetBehaviourState(DocileLocustBeesState.ROAMING);
        else
            enemy.SetBehaviourState(DocileLocustBeesState.HIDING);
    }

    public bool SyncAnimationSpeedEnabled(DocileLocustBeesAI enemy) => false;
}
