#region

using UnityEngine;

#endregion

class HygrodereController : IEnemyController<BlobAI> {
    readonly Vector3 camOffset = new(0, 2f, -3f);

    public Vector3 GetCameraOffset(BlobAI enemy) => this.camOffset;

    public void OnSecondarySkillHold(BlobAI enemy) {
        this.SetAngeredTimer(enemy, 0.0f);
        this.SetTamedTimer(enemy, 2.0f);
    }

    public void ReleaseSecondarySkill(BlobAI enemy) => this.SetTamedTimer(enemy, 0.0f);

    public void UsePrimarySkill(BlobAI enemy) => this.SetAngeredTimer(enemy, 18.0f);

    public float SprintMultiplier(BlobAI _) => 9.8f;

    public void OnOutsideStatusChange(BlobAI enemy) => enemy.StopSearch(enemy.searchForPlayers, true);


    void SetTamedTimer(BlobAI enemy, float time) => enemy.Reflect().SetInternalField("tamedTimer", time);

    void SetAngeredTimer(BlobAI enemy, float time) => enemy.Reflect().SetInternalField("angeredTimer", time);
}
