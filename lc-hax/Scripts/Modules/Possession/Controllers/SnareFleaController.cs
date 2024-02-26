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


    public Vector3 CamOffsets = new(0, 2.5f, -3f);

    public Vector3 GetCameraOffset(CentipedeAI enemy) {
        float targetCamOffsetY, targetCamOffsetZ;

        if (!enemy.IsBehaviourState(SnareFleaState.HIDING) && enemy.clingingToPlayer is null) { // Is Roaming
            this.transitionSpeed = 8.0f;
            targetCamOffsetY = 2f;
            targetCamOffsetZ = -4f;
        }
        else if (enemy.clingingToPlayer is not null) { // On Player
            this.transitionSpeed = 4.5f;
            targetCamOffsetY = 0f;
            targetCamOffsetZ = -2f;
        }
        else { // On Ceiling
            this.transitionSpeed = 2.5f;
            targetCamOffsetY = -0.3f;
            targetCamOffsetZ = 0f;
        }

        // Smoothly interpolate between current and target camera positions
        this.CamOffsets.y = Mathf.Lerp(this.CamOffsets.y, targetCamOffsetY, Time.deltaTime * this.transitionSpeed);
        this.CamOffsets.z = Mathf.Lerp(this.CamOffsets.z, targetCamOffsetZ, Time.deltaTime * this.transitionSpeed);

        return this.CamOffsets;
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
