#region

using UnityEngine;

#endregion

class LassoManController : IEnemyController<LassoManAI> {
    readonly Vector3 CamOffset = new(0, 2.8f, -3.5f);

    public Vector3 GetCameraOffset(LassoManAI enemy) => this.CamOffset;

    public bool SyncAnimationSpeedEnabled(LassoManAI enemy) => false;

    public void OnOutsideStatusChange(LassoManAI enemy) => enemy.StopSearch(enemy.searchForPlayers, true);
}
