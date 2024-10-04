using UnityEngine;

class LassoManController : IEnemyController<LassoManAI> {
    public Vector3 GetCameraOffset(LassoManAI enemy) => new(0.0f, 2.8f, -3.5f);

    public void UsePrimarySkill(LassoManAI enemy) => enemy.MakeScreechNoiseServerRpc();

    public bool SyncAnimationSpeedEnabled(LassoManAI enemy) => false;

    public void OnOutsideStatusChange(LassoManAI enemy) => enemy.StopSearch(enemy.searchForPlayers, true);
}

