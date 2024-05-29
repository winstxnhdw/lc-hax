using Hax;
using UnityEngine;

internal enum NutcrackerState
{
    WALKING,
    SENTRY
}

internal class NutcrackerController : IEnemyController<NutcrackerEnemyAI>
{
    private readonly Vector3 CamOffset = new(0, 2.8f, -3f);

    private bool InSentryMode { get; set; } = false;

    public Vector3 GetCameraOffset(NutcrackerEnemyAI enemy)
    {
        return CamOffset;
    }

    public void Update(NutcrackerEnemyAI enemy, bool isAIControlled)
    {
        if (isAIControlled) return;
        var Nutcracker = enemy.Reflect();

        var timeSinceFiringGun = Nutcracker.GetInternalField<float>("timeSinceFiringGun");
        var reloadingGun = Nutcracker.GetInternalField<bool>("reloadingGun");
        var aimingGun = Nutcracker.GetInternalField<bool>("aimingGun");

        if (timeSinceFiringGun > 0.75f && enemy.gun.shellsLoaded <= 0 && !reloadingGun && !aimingGun)
        {
            enemy.ReloadGunServerRpc();
            enemy.SetBehaviourState(NutcrackerState.WALKING);
            InSentryMode = false;
        }

        if (InSentryMode) return;
        enemy.SwitchToBehaviourServerRpc(unchecked((int)NutcrackerState.WALKING));
    }

    public bool IsAbleToRotate(NutcrackerEnemyAI enemy)
    {
        return !enemy.IsBehaviourState(NutcrackerState.SENTRY);
    }

    public bool IsAbleToMove(NutcrackerEnemyAI enemy)
    {
        return !enemy.IsBehaviourState(NutcrackerState.SENTRY);
    }

    public void UsePrimarySkill(NutcrackerEnemyAI enemy)
    {
        var reloadingGun = enemy.Reflect().GetInternalField<bool>("reloadingGun");
        if (enemy.gun is not ShotgunItem shotgun || enemy.gun.shellsLoaded <= 0 || reloadingGun) return;
        enemy.AimGunServerRpc(enemy.transform.forward);
        shotgun.gunShootAudio.volume = 0.25f;
        enemy.FireGunServerRpc();
    }

    public void OnSecondarySkillHold(NutcrackerEnemyAI enemy)
    {
        var reloadingGun = enemy.Reflect().GetInternalField<bool>("reloadingGun");
        if (reloadingGun) return;
        enemy.SetBehaviourState(NutcrackerState.SENTRY);
        InSentryMode = true;
    }

    public void ReleaseSecondarySkill(NutcrackerEnemyAI enemy)
    {
        enemy.SetBehaviourState(NutcrackerState.WALKING);
        InSentryMode = false;
    }

    public void UseSpecialAbility(NutcrackerEnemyAI enemy)
    {
        var reloadingGun = enemy.Reflect().GetInternalField<bool>("reloadingGun");
        var Nutcracker = enemy.Reflect();
        var SaveTimesSeeingSamePlayer = Nutcracker.GetInternalField<int>("timesSeeingSamePlayer");
        var SaveHP = enemy.enemyHP;
        var SaveShellsLoaded = enemy.gun.shellsLoaded;
        if (enemy.IsBehaviourState(NutcrackerState.WALKING))
        {
            _ = Nutcracker.SetInternalField("timesSeeingSamePlayer", 3);
            enemy.gun.shellsLoaded = 1;
            enemy.enemyHP = 1;
        }

        enemy.AimGunServerRpc(enemy.transform.position);
        _ = Nutcracker.SetInternalField("timesSeeingSamePlayer", SaveTimesSeeingSamePlayer);
        enemy.enemyHP = SaveHP;
        enemy.gun.shellsLoaded = SaveShellsLoaded;
    }

    public void OnUnpossess(NutcrackerEnemyAI enemy)
    {
        InSentryMode = false;
    }

    public string GetPrimarySkillName(NutcrackerEnemyAI enemy)
    {
        return enemy.gun is null ? "" : "Fire";
    }

    public string GetSecondarySkillName(NutcrackerEnemyAI _)
    {
        return "(HOLD) Sentry mode";
    }

    public void OnOutsideStatusChange(NutcrackerEnemyAI enemy)
    {
        enemy.StopSearch(enemy.attackSearch, true);
        enemy.StopSearch(enemy.patrol, true);
    }
}