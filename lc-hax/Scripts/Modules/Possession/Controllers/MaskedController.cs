internal class MaskedPlayerController : IEnemyController<MaskedPlayerEnemy> {
    internal void UseSecondarySkill(MaskedPlayerEnemy enemyInstance) =>
        enemyInstance.SetCrouchingServerRpc(!enemyInstance.Reflect().GetInternalField<bool>("crouching"));
}
