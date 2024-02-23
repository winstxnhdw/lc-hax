using GameNetcodeStuff;

internal class MaskedPlayerController : IEnemyController<MaskedPlayerEnemy> {

    void SetstartingKillAnimationLocalClient(MaskedPlayerEnemy enemy, bool value) =>
        enemy.Reflect().SetInternalField("startingKillAnimationLocalClient", value);

    bool GetinKillAnimation(MaskedPlayerEnemy enemy) => enemy.Reflect().GetInternalField<bool>("inKillAnimation");
    public void UsePrimarySkill(MaskedPlayerEnemy enemy) => enemy.SetHandsOutServerRpc(!enemy.creatureAnimator.GetBool("HandsOut"));

    public void UseSecondarySkill(MaskedPlayerEnemy enemy) => enemy.SetCrouchingServerRpc(!enemy.creatureAnimator.GetBool("Crouching"));

    public float InteractRange(MaskedPlayerEnemy _) => 1.0f;

    public bool SyncAnimationSpeedEnabled(MaskedPlayerEnemy _) => false;

    public void OnOutsideStatusChange(MaskedPlayerEnemy enemy) => enemy.StopSearch(enemy.searchForPlayers, true);


    public void OnPlayerCollision(MaskedPlayerEnemy enemy, PlayerControllerB player) {
        if(this.GetinKillAnimation(enemy)) return;
        PlayerControllerB playerControllerB = enemy.MeetsStandardPlayerCollisionConditions(player.playerCollider);
        if (playerControllerB != null) {

            this.SetstartingKillAnimationLocalClient(enemy, true);
            enemy.KillPlayerAnimationServerRpc((int)playerControllerB.actualClientId);
        }

    }
}
