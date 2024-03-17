using UnityEngine;

class MaskedPlayerController : IEnemyController<MaskedPlayerEnemy> {
    public Vector3 GetCameraOffset(MaskedPlayerEnemy enemy) => new(0.0f, 2.8f, -3.0f);

    public void UsePrimarySkill(MaskedPlayerEnemy enemy) => enemy.SetHandsOutServerRpc(!enemy.creatureAnimator.GetBool("HandsOut"));

    public void UseSecondarySkill(MaskedPlayerEnemy enemy) => enemy.SetCrouchingServerRpc(!enemy.creatureAnimator.GetBool("Crouching"));

    public bool IsAbleToMove(MaskedPlayerEnemy enemy) => !enemy.Reflect().GetInternalField<bool>("inKillAnimation");

    public bool IsAbleToRotate(MaskedPlayerEnemy enemy) => !enemy.Reflect().GetInternalField<bool>("inKillAnimation");

    public bool SyncAnimationSpeedEnabled(MaskedPlayerEnemy _) => false;

    public void OnOutsideStatusChange(MaskedPlayerEnemy enemy) => enemy.StopSearch(enemy.searchForPlayers, true);
}
