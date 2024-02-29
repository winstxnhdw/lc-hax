using UnityEngine;

internal class MaskedPlayerController : IEnemyController<MaskedPlayerEnemy> {

    Vector3 camOffset = new(0, 2.8f, -3f);

    public Vector3 GetCameraOffset(MaskedPlayerEnemy enemy) => this.camOffset;

    void SetstartingKillAnimationLocalClient(MaskedPlayerEnemy enemy, bool value) =>
        enemy.Reflect().SetInternalField("startingKillAnimationLocalClient", value);

    bool GetinKillAnimation(MaskedPlayerEnemy enemy) => enemy.Reflect().GetInternalField<bool>("inKillAnimation");

    public void UsePrimarySkill(MaskedPlayerEnemy enemy) => enemy.SetHandsOutServerRpc(!enemy.creatureAnimator.GetBool("HandsOut"));

    public void UseSecondarySkill(MaskedPlayerEnemy enemy) => enemy.SetCrouchingServerRpc(!enemy.creatureAnimator.GetBool("Crouching"));

    public bool IsAbleToMove(EnemyAI enemy) => !enemy.Reflect().GetInternalField<bool>("inKillAnimation");

    public bool IsAbleToRotate(EnemyAI enemy) => !enemy.Reflect().GetInternalField<bool>("inKillAnimation");

    public bool SyncAnimationSpeedEnabled(MaskedPlayerEnemy _) => false;

    public void OnOutsideStatusChange(MaskedPlayerEnemy enemy) => enemy.StopSearch(enemy.searchForPlayers, true);


}
