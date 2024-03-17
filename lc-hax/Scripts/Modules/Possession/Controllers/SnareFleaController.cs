using Hax;
using UnityEngine;

enum SnareFleaState {
    SEARCHING,
    HIDING,
    CHASING,
    CLINGING
}

class SnareFleaController : IEnemyController<CentipedeAI> {
    public Vector3 GetCameraOffset(CentipedeAI enemy) {
        Vector3 cameraOffset = new(0.0f, 2.5f, -3.0f);
        float transitionSpeed = 2.5f;
        float targetCamOffsetY = 2.5f;
        float targetCamOffsetZ = 0.0f;

        // Snare Flea is roaming
        if (!enemy.IsBehaviourState(SnareFleaState.HIDING) && enemy.clingingToPlayer is null) {
            transitionSpeed = 8.0f;
            targetCamOffsetY = 2.0f;
            targetCamOffsetZ = -4.0f;
        }

        else if (enemy.clingingToPlayer is not null) {
            transitionSpeed = 4.5f;
            targetCamOffsetY = 0.0f;
            targetCamOffsetZ = -2.0f;
        }

        float transitionSpeedDelta = transitionSpeed * Time.deltaTime;
        cameraOffset.y = Mathf.Lerp(cameraOffset.y, targetCamOffsetY, transitionSpeedDelta);
        cameraOffset.z = Mathf.Lerp(cameraOffset.z, targetCamOffsetZ, transitionSpeedDelta);

        return cameraOffset;
    }

    bool IsClingingToSomething(CentipedeAI enemy) {
        Reflector<CentipedeAI> centipedeReflector = enemy.Reflect();

        return enemy.clingingToPlayer is not null || enemy.inSpecialAnimation ||
               centipedeReflector.GetInternalField<bool>("clingingToDeadBody") ||
               centipedeReflector.GetInternalField<bool>("clingingToCeiling") ||
               centipedeReflector.GetInternalField<bool>("startedCeilingAnimationCoroutine") ||
               centipedeReflector.GetInternalField<bool>("inDroppingOffPlayerAnim");
    }

    public void UsePrimarySkill(CentipedeAI enemy) {
        if (!enemy.IsBehaviourState(SnareFleaState.HIDING)) return;
        enemy.SetBehaviourState(SnareFleaState.CHASING);
    }

    public void UseSecondarySkill(CentipedeAI enemy) {
        if (this.IsClingingToSomething(enemy)) return;

        _ = enemy.Reflect().InvokeInternalMethod("RaycastToCeiling");
        enemy.SetBehaviourState(SnareFleaState.HIDING);
    }

    public bool IsAbleToMove(CentipedeAI enemy) => !this.IsClingingToSomething(enemy);

    public bool IsAbleToRotate(CentipedeAI enemy) => !this.IsClingingToSomething(enemy);

    public string GetPrimarySkillName(CentipedeAI _) => "Drop";

    public string GetSecondarySkillName(CentipedeAI _) => "Attach to ceiling";

    public bool CanUseEntranceDoors(CentipedeAI _) => false;

    public bool SyncAnimationSpeedEnabled(CentipedeAI _) => false;
}
