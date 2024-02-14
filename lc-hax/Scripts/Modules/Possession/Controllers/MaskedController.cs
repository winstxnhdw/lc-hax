internal class MaskedPlayerController : IEnemyController<MaskedPlayerEnemy> {
    public void UsePrimarySkill(MaskedPlayerEnemy enemyInstance) => enemyInstance.SetHandsOutServerRpc(!enemyInstance.creatureAnimator.GetBool("HandsOut"));

    public void UseSecondarySkill(MaskedPlayerEnemy enemyInstance) => enemyInstance.SetCrouchingServerRpc(!enemyInstance.creatureAnimator.GetBool("Crouching"));

    public bool CanUseEntranceDoors(MaskedPlayerEnemy _) => true;

}
