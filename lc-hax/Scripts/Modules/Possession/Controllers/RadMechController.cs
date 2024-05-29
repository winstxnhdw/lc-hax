using Hax;
using UnityEngine;

public enum RadMechBehaviorState
{
    Default,
    Alert,
    Flying
}

internal class RadMechController : IEnemyController<RadMechAI>
{
    private readonly Vector3 camOffset = new(0, 8f, -8f);

    private bool isFiring = false;
    private float shootTimer = 0f;

    public void OnMovement(RadMechAI enemy, bool isMoving, bool isSprinting)
    {
        var inFlyingMode = enemy.Reflect().GetInternalField<bool>("inFlyingMode");
        if (inFlyingMode) return;
        if (isSprinting)
            enemy.timeBetweenSteps = 0.2f;
        else
            enemy.timeBetweenSteps = 0.7f;
    }

    public Vector3 GetCameraOffset(RadMechAI enemy)
    {
        return camOffset;
    }

    public bool IsAbleToMove(RadMechAI enemy)
    {
        return !enemy.inTorchPlayerAnimation || !isFiring;
    }

    public bool CanUseEntranceDoors(RadMechAI _)
    {
        return false;
    }

    public float InteractRange(RadMechAI _)
    {
        return 0f;
    }

    public bool SyncAnimationSpeedEnabled(RadMechAI _)
    {
        return false;
    }

    public void OnOutsideStatusChange(RadMechAI enemy)
    {
        enemy.StopSearch(enemy.searchForPlayers, true);
    }

    public void UseSecondarySkill(RadMechAI enemy)
    {
        enemy.SetBehaviourState(RadMechBehaviorState.Alert);
        if (!enemy.spotlight.activeSelf)
            enemy.EnableSpotlight();
        else
            enemy.DisableSpotlight();
    }


    // set special ability to flying mode
    public void UseSpecialAbility(RadMechAI enemy)
    {
        var inFlyingMode = enemy.Reflect().GetInternalField<bool>("inFlyingMode");
        if (!inFlyingMode)
        {
            enemy.SetBehaviourState(RadMechBehaviorState.Flying);
            enemy.StartFlying();
        }
        else
        {
            enemy.EndFlight();
        }
    }


    public void OnPrimarySkillHold(RadMechAI enemy)
    {
        enemy.SetBehaviourState(RadMechBehaviorState.Alert);
        var player = enemy.FindClosestPlayer(4f);
        enemy.targetPlayer = player;
        enemy.targetedThreat = player.ToThreat();

        GetCurrentTarget(enemy);
        isFiring = true;
    }

    public void ReleasePrimarySkill(RadMechAI enemy)
    {
        if (isFiring)
        {
            enemy.SetBehaviourState(RadMechBehaviorState.Default);
            enemy.SetAimingGun(false);
            isFiring = false;
        }
    }

    public bool isHostOnly(RadMechAI _)
    {
        return true;
    }

    public void OnUnpossess(RadMechAI _)
    {
        isFiring = false;
    }

    public void Update(RadMechAI enemy, bool isAIControlled)
    {
        if (isAIControlled) return;
        if (isFiring)
        {
            shootTimer += Time.deltaTime;
            if (!enemy.aimingGun) enemy.SetAimingGun(true);
            if (shootTimer >= enemy.fireRate)
            {
                shootTimer = 0f;
                enemy.StartShootGun();
            }
        }
    }

    public void GetCurrentTarget(RadMechAI enemy)
    {
        // if we have a player to target, else look for one
        if (enemy.targetPlayer is not null)
        {
            enemy.targetedThreat = enemy.targetPlayer.ToThreat();
        }
        else
        {
            enemy.targetPlayer = enemy.FindClosestPlayer(4f);
            enemy.targetedThreat = enemy.targetPlayer.ToThreat();
        }
    }
}