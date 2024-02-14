internal interface IController {

    void OnPossess(EnemyAI instance);
    void OnUnpossess(EnemyAI instance);
    void OnDeath(EnemyAI instance);

    void Update(EnemyAI enemyInstance);

    void UsePrimarySkill(EnemyAI enemyInstance);

    void UseSecondarySkill(EnemyAI enemyInstance);

    void ReleaseSecondarySkill(EnemyAI enemyInstance);

    void OnMovement(EnemyAI enemyInstance, bool isMoving, bool isSprinting);

    bool IsAbleToMove(EnemyAI enemyInstance);

    bool IsAbleToRotate(EnemyAI enemyInstance);
    bool CanUseEntranceDoors(EnemyAI enemyInstance);

    string? GetPrimarySkillName(EnemyAI enemyInstance);

    string? GetSecondarySkillName(EnemyAI enemyInstance);


}

internal interface IEnemyController<T> : IController where T : EnemyAI {
    void OnPossess(T instance) { }
    void OnUnpossess(T instance) { }
    void OnDeath(T instance) { }
    void Update(T enemyInstance) { }

    void UsePrimarySkill(T enemyInstance) { }

    void UseSecondarySkill(T enemyInstance) { }

    void ReleaseSecondarySkill(T enemyInstance) { }

    void OnMovement(T enemyInstance, bool isMoving, bool isSprinting) { }

    bool IsAbleToMove(T enemyInstance) => true;

    bool IsAbleToRotate(T enemyInstance) => true;

    bool CanUseEntranceDoors(T enemyInstance) => true;

    string? GetPrimarySkillName(T enemyInstance) => null;

    string? GetSecondarySkillName(T enemyInstance) => null;

    void IController.OnPossess(EnemyAI instance) => this.OnPossess((T)instance);
    void IController.OnUnpossess(EnemyAI instance) => this.OnUnpossess((T)instance);
    void IController.OnDeath(EnemyAI instance) => this.OnDeath((T)instance);

    void IController.Update(EnemyAI enemyInstance) => this.Update((T)enemyInstance);

    void IController.UsePrimarySkill(EnemyAI enemyInstance) => this.UsePrimarySkill((T)enemyInstance);

    void IController.UseSecondarySkill(EnemyAI enemyInstance) => this.UseSecondarySkill((T)enemyInstance);

    void IController.ReleaseSecondarySkill(EnemyAI enemyInstance) => this.ReleaseSecondarySkill((T)enemyInstance);

    void IController.OnMovement(EnemyAI enemyInstance, bool isMoving, bool isSprinting) => this.OnMovement((T)enemyInstance, isMoving, isSprinting);

    bool IController.IsAbleToMove(EnemyAI enemyInstance) => this.IsAbleToMove((T)enemyInstance);

    bool IController.IsAbleToRotate(EnemyAI enemyInstance) => this.IsAbleToRotate((T)enemyInstance);
    bool IController.CanUseEntranceDoors(EnemyAI enemyInstance) => this.CanUseEntranceDoors((T)enemyInstance);

    string? IController.GetPrimarySkillName(EnemyAI enemyInstance) => this.GetPrimarySkillName((T)enemyInstance);

    string? IController.GetSecondarySkillName(EnemyAI enemyInstance) => this.GetSecondarySkillName((T)enemyInstance);
}
