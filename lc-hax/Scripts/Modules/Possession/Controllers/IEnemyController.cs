using UnityEngine;

internal interface IController
{
    const float DefaultSprintMultiplier = 2.8f;

    const float DefaultInteractRange = 4.5f;

    static Vector3 DefaultCamOffsets => new(0, 2.5f, -3f);

    static Vector3 DefaultEnemyOffset => new();

    Vector3 GetCameraOffset(EnemyAI enemy);

    Vector3 GetEnemyPositionOffset(EnemyAI enemy);

    void OnEnableAIControl(EnemyAI enemy, bool enabled);

    void OnPossess(EnemyAI enemy);

    void OnUnpossess(EnemyAI enemy);

    void OnDeath(EnemyAI enemy);

    void Update(EnemyAI enemy, bool isAIControlled);

    void LateUpdate(EnemyAI enemy);

    void UsePrimarySkill(EnemyAI enemy);
    void OnPrimarySkillHold(EnemyAI enemy);

    void ReleasePrimarySkill(EnemyAI enemy);

    void OnSecondarySkillHold(EnemyAI enemy);

    void UseSecondarySkill(EnemyAI enemy);

    void ReleaseSecondarySkill(EnemyAI enemy);

    void UseSpecialAbility(EnemyAI enemy);

    void OnMovement(EnemyAI enemy, bool isMoving, bool isSprinting);

    bool IsAbleToMove(EnemyAI enemy);

    bool IsAbleToRotate(EnemyAI enemy);

    bool CanUseEntranceDoors(EnemyAI enemy);

    string? GetPrimarySkillName(EnemyAI enemy);

    string? GetSecondarySkillName(EnemyAI enemy);

    float InteractRange(EnemyAI enemy);

    bool isHostOnly(EnemyAI enemy);

    float SprintMultiplier(EnemyAI enemy);

    bool SyncAnimationSpeedEnabled(EnemyAI enemy);

    void OnOutsideStatusChange(EnemyAI enemy);
}

/// <summary>
///     Do Not forget to register the controller in the <see cref="PossessionMod.EnemyControllers" />
/// </summary>
/// <typeparam name="T"></typeparam>
internal interface IEnemyController<T> : IController where T : EnemyAI
{
    void IController.OnEnableAIControl(EnemyAI enemy, bool enabled)
    {
        OnEnableAIControl((T)enemy, enabled);
    }

    void IController.OnPossess(EnemyAI enemy)
    {
        OnPossess((T)enemy);
    }

    void IController.OnUnpossess(EnemyAI enemy)
    {
        OnUnpossess((T)enemy);
    }

    void IController.OnDeath(EnemyAI enemy)
    {
        OnDeath((T)enemy);
    }

    void IController.Update(EnemyAI enemy, bool isAIControlled)
    {
        Update((T)enemy, isAIControlled);
    }

    void IController.LateUpdate(EnemyAI enemy)
    {
        LateUpdate((T)enemy);
    }

    void IController.UsePrimarySkill(EnemyAI enemy)
    {
        UsePrimarySkill((T)enemy);
    }

    void IController.OnPrimarySkillHold(EnemyAI enemy)
    {
        OnPrimarySkillHold((T)enemy);
    }

    void IController.ReleasePrimarySkill(EnemyAI enemy)
    {
        ReleasePrimarySkill((T)enemy);
    }

    void IController.OnSecondarySkillHold(EnemyAI enemy)
    {
        OnSecondarySkillHold((T)enemy);
    }

    void IController.UseSecondarySkill(EnemyAI enemy)
    {
        UseSecondarySkill((T)enemy);
    }

    void IController.ReleaseSecondarySkill(EnemyAI enemy)
    {
        ReleaseSecondarySkill((T)enemy);
    }

    void IController.UseSpecialAbility(EnemyAI enemy)
    {
        UseSpecialAbility((T)enemy);
    }

    void IController.OnMovement(EnemyAI enemy, bool isMoving, bool isSprinting)
    {
        OnMovement((T)enemy, isMoving, isSprinting);
    }

    bool IController.IsAbleToMove(EnemyAI enemy)
    {
        return IsAbleToMove((T)enemy);
    }

    bool IController.IsAbleToRotate(EnemyAI enemy)
    {
        return IsAbleToRotate((T)enemy);
    }

    bool IController.CanUseEntranceDoors(EnemyAI enemy)
    {
        return CanUseEntranceDoors((T)enemy);
    }

    string? IController.GetPrimarySkillName(EnemyAI enemy)
    {
        return GetPrimarySkillName((T)enemy);
    }

    string? IController.GetSecondarySkillName(EnemyAI enemy)
    {
        return GetSecondarySkillName((T)enemy);
    }

    bool IController.isHostOnly(EnemyAI enemy)
    {
        return isHostOnly((T)enemy);
    }

    float IController.InteractRange(EnemyAI enemy)
    {
        return InteractRange((T)enemy);
    }

    float IController.SprintMultiplier(EnemyAI enemy)
    {
        return SprintMultiplier((T)enemy);
    }

    Vector3 IController.GetCameraOffset(EnemyAI enemy)
    {
        return GetCameraOffset((T)enemy);
    }

    Vector3 IController.GetEnemyPositionOffset(EnemyAI enemy)
    {
        return GetEnemyPositionOffset((T)enemy);
    }

    bool IController.SyncAnimationSpeedEnabled(EnemyAI enemy)
    {
        return SyncAnimationSpeedEnabled((T)enemy);
    }

    void IController.OnOutsideStatusChange(EnemyAI enemy)
    {
        OnOutsideStatusChange((T)enemy);
    }

    void OnEnableAIControl(T enemy, bool enabled)
    {
    }

    void OnPossess(T enemy)
    {
    }

    void OnUnpossess(T enemy)
    {
    }

    void OnDeath(T enemy)
    {
    }

    void Update(T enemy, bool isAIControlled)
    {
    }

    void LateUpdate(T enemy)
    {
    }

    void UsePrimarySkill(T enemy)
    {
    }

    void OnPrimarySkillHold(T enemy)
    {
    }

    void ReleasePrimarySkill(T enemy)
    {
    }

    void OnSecondarySkillHold(T enemy)
    {
    }

    void UseSecondarySkill(T enemy)
    {
    }

    void ReleaseSecondarySkill(T enemy)
    {
    }

    void UseSpecialAbility(T enemy)
    {
    }

    void OnMovement(T enemy, bool isMoving, bool isSprinting)
    {
    }

    bool IsAbleToMove(T enemy)
    {
        return true;
    }

    bool IsAbleToRotate(T enemy)
    {
        return true;
    }

    bool CanUseEntranceDoors(T enemy)
    {
        return true;
    }

    string? GetPrimarySkillName(T enemy)
    {
        return null;
    }

    string? GetSecondarySkillName(T enemy)
    {
        return null;
    }

    bool isHostOnly(T enemy)
    {
        return false;
    }

    float InteractRange(T enemy)
    {
        return DefaultInteractRange;
    }

    float SprintMultiplier(T enemy)
    {
        return DefaultSprintMultiplier;
    }

    Vector3 GetCameraOffset(T enemy)
    {
        return DefaultCamOffsets;
    }

    Vector3 GetEnemyPositionOffset(T enemy)
    {
        return DefaultEnemyOffset;
    }

    bool SyncAnimationSpeedEnabled(T enemy)
    {
        return true;
    }

    void OnOutsideStatusChange(T enemy)
    {
    }
}