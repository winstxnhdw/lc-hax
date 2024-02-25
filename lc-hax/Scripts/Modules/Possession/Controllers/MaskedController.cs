using GameNetcodeStuff;

internal class MaskedPlayerController : IEnemyController<MaskedPlayerEnemy> {
    public void GetCameraPosition(MaskedPlayerEnemy enemy) {
        PossessionMod.CamOffsetY = 2.8f;
        PossessionMod.CamOffsetZ = -3f;
    }

    void SetstartingKillAnimationLocalClient(MaskedPlayerEnemy enemy, bool value) =>
        enemy.Reflect().SetInternalField("startingKillAnimationLocalClient", value);

    bool GetinKillAnimation(MaskedPlayerEnemy enemy) => enemy.Reflect().GetInternalField<bool>("inKillAnimation");
    public void UsePrimarySkill(MaskedPlayerEnemy enemy) => enemy.SetHandsOutServerRpc(!enemy.creatureAnimator.GetBool("HandsOut"));

    public void UseSecondarySkill(MaskedPlayerEnemy enemy) => enemy.SetCrouchingServerRpc(!enemy.creatureAnimator.GetBool("Crouching"));

    public float InteractRange(MaskedPlayerEnemy _) => 1.0f;

    public bool SyncAnimationSpeedEnabled(MaskedPlayerEnemy _) => false;

    public void OnOutsideStatusChange(MaskedPlayerEnemy enemy) => enemy.StopSearch(enemy.searchForPlayers, true);


    public void OnCollideWithPlayer(MaskedPlayerEnemy enemy, PlayerControllerB player) {

    }
}
