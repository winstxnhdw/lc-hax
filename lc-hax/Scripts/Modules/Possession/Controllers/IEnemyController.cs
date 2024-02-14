internal interface IController {

    void OnPossess(EnemyAI enemyInstance);

    void OnUnpossess(EnemyAI enemyInstance);
    void OnDeath(EnemyAI enemyInstance);

    void Update(EnemyAI enemyInstance);

    void UsePrimarySkill(EnemyAI enemyInstance);

    void UseSecondarySkill(EnemyAI enemyInstance);

    void ReleaseSecondarySkill(EnemyAI enemyInstance);

    void OnMovement(EnemyAI enemyInstance, bool isMoving, bool isSprinting);

    bool IsAbleToMove(EnemyAI enemyInstance);

    bool IsAbleToRotate(EnemyAI enemyInstance);

    string? GetPrimarySkillName(EnemyAI enemyInstance);

    string? GetSecondarySkillName(EnemyAI enemyInstance);

    void UpdateCharacter(EnemyAI instance, CharacterMovement movement);
}

internal interface IEnemyController<T> : IController where T : EnemyAI {
    void OnPossess(T enemyInstance) { }
    void OnUnpossess(T enemyInstance) { }
    void OnDeath(T enemyInstance) { }

    void Update(T enemyInstance) { }

    void UsePrimarySkill(T enemyInstance) { }

    void UseSecondarySkill(T enemyInstance) { }

    void ReleaseSecondarySkill(T enemyInstance) { }

    void OnMovement(T enemyInstance, bool isMoving, bool isSprinting) { }

    bool IsAbleToMove(T enemyInstance) => true;

    bool IsAbleToRotate(T enemyInstance) => true;

    string? GetPrimarySkillName(T enemyInstance) => null;

    string? GetSecondarySkillName(T enemyInstance) => null;
    void IController.OnPossess(EnemyAI enemyInstance) => this.OnPossess((T)enemyInstance);
    void IController.OnUnpossess(EnemyAI enemyInstance) => this.OnUnpossess((T)enemyInstance);
    void IController.OnDeath(EnemyAI enemyInstance) => this.OnDeath((T)enemyInstance);

    void IController.Update(EnemyAI enemyInstance) => this.Update((T)enemyInstance);

    void IController.UsePrimarySkill(EnemyAI enemyInstance) => this.UsePrimarySkill((T)enemyInstance);

    void IController.UseSecondarySkill(EnemyAI enemyInstance) => this.UseSecondarySkill((T)enemyInstance);

    void IController.ReleaseSecondarySkill(EnemyAI enemyInstance) => this.ReleaseSecondarySkill((T)enemyInstance);

    void IController.OnMovement(EnemyAI enemyInstance, bool isMoving, bool isSprinting) => this.OnMovement((T)enemyInstance, isMoving, isSprinting);

    bool IController.IsAbleToMove(EnemyAI enemyInstance) => this.IsAbleToMove((T)enemyInstance);

    bool IController.IsAbleToRotate(EnemyAI enemyInstance) => this.IsAbleToRotate((T)enemyInstance);

    string? IController.GetPrimarySkillName(EnemyAI enemyInstance) => this.GetPrimarySkillName((T)enemyInstance);

    string? IController.GetSecondarySkillName(EnemyAI enemyInstance) => this.GetSecondarySkillName((T)enemyInstance);

    void IController.UpdateCharacter(EnemyAI instance, CharacterMovement movement) => this.UpdateCharacter((T)instance, movement);
}
