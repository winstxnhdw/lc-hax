public class MaskedPlayerController : IEnemyController<MaskedPlayerEnemy> {
    public void UseSecondarySkill(MaskedPlayerEnemy enemyInstance) =>
        enemyInstance.SetCrouchingServerRpc(!enemyInstance.Reflect().GetInternalField<bool>("crouching"));
}
