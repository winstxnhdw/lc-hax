using GameNetcodeStuff;
using Hax;
using UnityEngine;

enum SnareFleaState {
    SEARCHING,
    HIDING,
    CHASING,
    CLINGING
}

internal class SnareFleaController : IEnemyController<CentipedeAI> {
    public float transitionSpeed = 0f;

    public void GetCameraPosition(CentipedeAI enemy) {
        float targetCamOffsetY, targetCamOffsetZ;

        if (!enemy.IsBehaviourState(SnareFleaState.HIDING)) {
            transitionSpeed = 8.0f;
            targetCamOffsetY = 2f;
            targetCamOffsetZ = -4f;
        }
        else {
            transitionSpeed = 2.5f;
            targetCamOffsetY = -0.3f;
            targetCamOffsetZ = 0f;
        }

        // Smoothly interpolate between current and target camera positions
        PossessionMod.CamOffsetY = Mathf.Lerp(PossessionMod.CamOffsetY, targetCamOffsetY, Time.deltaTime * transitionSpeed);
        PossessionMod.CamOffsetZ = Mathf.Lerp(PossessionMod.CamOffsetZ, targetCamOffsetZ, Time.deltaTime * transitionSpeed);
    }

    bool IsClingingToSomething(CentipedeAI enemy) {
        Reflector centipedeReflector = enemy.Reflect();

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

    public float InteractRange(CentipedeAI _) => 1.5f;

    public bool CanUseEntranceDoors(CentipedeAI _) => false;

    public bool SyncAnimationSpeedEnabled(CentipedeAI _) => false;

    public void OnCollideWithPlayer(CentipedeAI enemy, PlayerControllerB player) => enemy.OnCollideWithPlayer(player.playerCollider);
}
