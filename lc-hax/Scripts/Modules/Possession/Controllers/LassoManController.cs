using UnityEngine;

internal class LassoManController : IEnemyController<LassoManAI> {

    Vector3 CamOffset = new Vector3(0, 2.8f, -3.5f);

    public Vector3 GetCameraOffset(LassoManAI enemy) => this.CamOffset;

    public void UsePrimarySkill(LassoManAI enemy) => enemy.MakeScreechNoiseServerRpc();

    public bool SyncAnimationSpeedEnabled(LassoManAI enemy) => false;

    public void OnOutsideStatusChange(LassoManAI enemy) => enemy.StopSearch(enemy.searchForPlayers, true);

}

