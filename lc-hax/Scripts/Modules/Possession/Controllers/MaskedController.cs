#region

using UnityEngine;

#endregion

class MaskedPlayerController : IEnemyController<MaskedPlayerEnemy> {
    readonly Vector3 camOffset = new(0, 2.8f, -3f);

    public Vector3 GetCameraOffset(MaskedPlayerEnemy enemy) => this.camOffset;

    public void UsePrimarySkill(MaskedPlayerEnemy enemy) =>
        enemy.SetHandsOutServerRpc(!enemy.Reflect().GetInternalField<bool>("HandsOut"));

    public void UseSecondarySkill(MaskedPlayerEnemy enemy) =>
        enemy.SetCrouchingServerRpc(!enemy.Reflect().GetInternalField<bool>("crouching"));

    public bool IsAbleToMove(MaskedPlayerEnemy enemy) => !enemy.Reflect().GetInternalField<bool>("inKillAnimation");

    public bool IsAbleToRotate(MaskedPlayerEnemy enemy) => !enemy.Reflect().GetInternalField<bool>("inKillAnimation");

    public bool SyncAnimationSpeedEnabled(MaskedPlayerEnemy _) => false;

    public void OnOutsideStatusChange(MaskedPlayerEnemy enemy) => enemy.StopSearch(enemy.searchForPlayers, true);

    public string GetPrimarySkillName(MaskedPlayerEnemy enemy) =>
        enemy.Reflect().GetInternalField<bool>("HandsOut") ? "Put hands down" : "Put hands up";

    public string GetSecondarySkillName(MaskedPlayerEnemy enemy) =>
        enemy.Reflect().GetInternalField<bool>("crouching") ? "Stand up" : "Crouch down";

}
