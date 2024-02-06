internal class MaskedPlayerController : IEnemyController<MaskedPlayerEnemy> {
    internal void UsePrimarySkill(MaskedPlayerEnemy enemyInstance) => enemyInstance.SetHandsOutServerRpc(!enemyInstance.creatureAnimator.GetBool("HandsOut"));

    internal void UseSecondarySkill(MaskedPlayerEnemy enemyInstance) => enemyInstance.SetCrouchingServerRpc(!enemyInstance.creatureAnimator.GetBool("Crouching"));
}
