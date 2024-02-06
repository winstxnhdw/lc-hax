internal interface IController {
    void Update(EnemyAI enemyInstance);

    void UsePrimarySkill(EnemyAI enemyInstance);

    void UseSecondarySkill(EnemyAI enemyInstance);

    void ReleaseSecondarySkill(EnemyAI enemyInstance);

    void OnMovement(EnemyAI enemyInstance);

    bool IsAbleToMove(EnemyAI enemyInstance);

    CharArray GetPrimarySkillName(EnemyAI enemyInstance);

    CharArray GetSecondarySkillName(EnemyAI enemyInstance);
}

internal interface IEnemyController<T> : IController where T : EnemyAI {
    void Update(T enemyInstance) { }

    void UsePrimarySkill(T enemyInstance) { }

    void UseSecondarySkill(T enemyInstance) { }

    void ReleaseSecondarySkill(T enemyInstance) { }

    void OnMovement(T enemyInstance) { }

    bool IsAbleToMove(T enemyInstance) => true;

    CharArray GetPrimarySkillName(T enemyInstance) => default;

    CharArray GetSecondarySkillName(T enemyInstance) => default;

    void IController.Update(EnemyAI enemyInstance) => this.Update((T)enemyInstance);

    void IController.UsePrimarySkill(EnemyAI enemyInstance) => this.UsePrimarySkill((T)enemyInstance);

    void IController.UseSecondarySkill(EnemyAI enemyInstance) => this.UseSecondarySkill((T)enemyInstance);

    void IController.ReleaseSecondarySkill(EnemyAI enemyInstance) => this.ReleaseSecondarySkill((T)enemyInstance);

    void IController.OnMovement(EnemyAI enemyInstance) => this.OnMovement((T)enemyInstance);

    bool IController.IsAbleToMove(EnemyAI enemyInstance) => this.IsAbleToMove((T)enemyInstance);

    CharArray IController.GetPrimarySkillName(EnemyAI enemyInstance) => this.GetPrimarySkillName((T)enemyInstance);

    CharArray IController.GetSecondarySkillName(EnemyAI enemyInstance) => this.GetSecondarySkillName((T)enemyInstance);
}
