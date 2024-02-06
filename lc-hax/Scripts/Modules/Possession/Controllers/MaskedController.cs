internal class MaskedPlayerController : IEnemyController<MaskedPlayerEnemy> {
    internal void UseSecondarySkill(MaskedPlayerEnemy enemyInstance) =>
        enemyInstance.SetCrouchingServerRpc(!enemyInstance.Reflect().GetInternalField<bool>("crouching"));


    internal void UsePrimarySkill(MaskedPlayerEnemy enemyInstance) {
        enemyInstance.SetHandsOutServerRpc(!enemyInstance.creatureAnimator.GetBool("HandsOut"));
    }
}
