#region

using Hax;
using UnityEngine;

#endregion

public enum SurgeonState {
    Idle,
    Searching,
    Chasing,
    Attacking,
    Dead
}

class ClaySurgeonController : IEnemyController<ClaySurgeonAI> {
    readonly Vector3 camOffset = new(0, 2.8f, -3f);

    public Vector3 GetCameraOffset(ClaySurgeonAI enemy) => this.camOffset;

    public void UsePrimarySkill(ClaySurgeonAI enemy) => enemy.DoBeatServerRpc();

    public string? GetPrimarySkillName(ClaySurgeonAI enemy) => "Dance Beat";


    public void OnOutsideStatusChange(ClaySurgeonAI enemy) {
        enemy.StopSearch(enemy.searchRoutine, true);
    }

    public void Update(ClaySurgeonAI enemy, bool isAIControlled) {
        var mat = enemy.Reflect().GetInternalField<Material>("thisMaterial");
        if(mat == null) return;
        mat.SetFloat("_AlphaCutoff", 0);
    }

    public bool CanUseEntranceDoors(ClaySurgeonAI enemy) => true;

    public bool SyncAnimationSpeedEnabled(ClaySurgeonAI enemy) => false;
}
