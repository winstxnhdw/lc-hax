using UnityEngine;

interface IController {
    const float DefaultSprintMultiplier = 2.8f;

    const float DefaultInteractRange = 4.5f;

    static Vector3 DefaultCamOffsets => new(0, 2.5f, -3f);

    static Vector3 DefaultEnemyOffset => new();

    Vector3 GetCameraOffset(EnemyAI enemy);

    Vector3 GetEnemyPositionOffset(EnemyAI enemy);

    void OnPossess(EnemyAI enemy);

    void OnUnpossess(EnemyAI enemy);

    void OnDeath(EnemyAI enemy);

    void OnMovement(EnemyAI enemy, bool isMoving, bool isSprinting);

    void Update(EnemyAI enemy, bool isAIControlled);

    void UsePrimarySkill(EnemyAI enemy);

    void OnSecondarySkillHold(EnemyAI enemy);

    void OnOutsideStatusChange(EnemyAI enemy);

    void UseSecondarySkill(EnemyAI enemy);

    void ReleaseSecondarySkill(EnemyAI enemy);

    void UseSpecialAbility(EnemyAI enemy);

    bool IsAbleToMove(EnemyAI enemy);

    bool IsAbleToRotate(EnemyAI enemy);

    bool CanUseEntranceDoors(EnemyAI enemy);

    string? GetPrimarySkillName(EnemyAI enemy);

    string? GetSecondarySkillName(EnemyAI enemy);

    float InteractRange(EnemyAI enemy);

    float SprintMultiplier(EnemyAI enemy);

    bool SyncAnimationSpeedEnabled(EnemyAI enemy);
}

interface IEnemyController<T> : IController where T : EnemyAI {
    void OnPossess(T enemy) { }

    void OnUnpossess(T enemy) { }

    void OnDeath(T enemy) { }

    void Update(T enemy, bool isAIControlled) { }

    void UsePrimarySkill(T enemy) { }

    void OnSecondarySkillHold(T enemy) { }

    void UseSecondarySkill(T enemy) { }

    void ReleaseSecondarySkill(T enemy) { }

    void UseSpecialAbility(T enemy) { }

    void OnOutsideStatusChange(T enemy) { }

    void OnMovement(T enemy, bool isMoving, bool isSprinting) { }

    bool IsAbleToMove(T enemy) => true;

    bool IsAbleToRotate(T enemy) => true;

    bool CanUseEntranceDoors(T enemy) => true;

    bool SyncAnimationSpeedEnabled(T enemy) => true;

    string? GetPrimarySkillName(T enemy) => null;

    string? GetSecondarySkillName(T enemy) => null;

    float InteractRange(T enemy) => IController.DefaultInteractRange;

    float SprintMultiplier(T enemy) => IController.DefaultSprintMultiplier;

    Vector3 GetCameraOffset(T enemy) => IController.DefaultCamOffsets;

    Vector3 GetEnemyPositionOffset(T enemy) => IController.DefaultEnemyOffset;

    void IController.OnPossess(EnemyAI enemy) => this.OnPossess((T)enemy);

    void IController.OnUnpossess(EnemyAI enemy) => this.OnUnpossess((T)enemy);

    void IController.OnDeath(EnemyAI enemy) => this.OnDeath((T)enemy);

    void IController.Update(EnemyAI enemy, bool isAIControlled) => this.Update((T)enemy, isAIControlled);

    void IController.UsePrimarySkill(EnemyAI enemy) => this.UsePrimarySkill((T)enemy);

    void IController.OnSecondarySkillHold(EnemyAI enemy) => this.OnSecondarySkillHold((T)enemy);

    void IController.UseSecondarySkill(EnemyAI enemy) => this.UseSecondarySkill((T)enemy);

    void IController.ReleaseSecondarySkill(EnemyAI enemy) => this.ReleaseSecondarySkill((T)enemy);

    void IController.UseSpecialAbility(EnemyAI enemy) => this.UseSpecialAbility((T)enemy);

    void IController.OnMovement(EnemyAI enemy, bool isMoving, bool isSprinting) => this.OnMovement((T)enemy, isMoving, isSprinting);

    bool IController.IsAbleToMove(EnemyAI enemy) => this.IsAbleToMove((T)enemy);

    bool IController.IsAbleToRotate(EnemyAI enemy) => this.IsAbleToRotate((T)enemy);

    bool IController.CanUseEntranceDoors(EnemyAI enemy) => this.CanUseEntranceDoors((T)enemy);

    string? IController.GetPrimarySkillName(EnemyAI enemy) => this.GetPrimarySkillName((T)enemy);

    string? IController.GetSecondarySkillName(EnemyAI enemy) => this.GetSecondarySkillName((T)enemy);

    float IController.InteractRange(EnemyAI enemy) => this.InteractRange((T)enemy);

    float IController.SprintMultiplier(EnemyAI enemy) => this.SprintMultiplier((T)enemy);

    Vector3 IController.GetCameraOffset(EnemyAI enemy) => this.GetCameraOffset((T)enemy);

    Vector3 IController.GetEnemyPositionOffset(EnemyAI enemy) => this.GetEnemyPositionOffset((T)enemy);

    bool IController.SyncAnimationSpeedEnabled(EnemyAI enemy) => this.SyncAnimationSpeedEnabled((T)enemy);

    void IController.OnOutsideStatusChange(EnemyAI enemy) => this.OnOutsideStatusChange((T)enemy);
}
