using Hax;
using UnityEngine;

enum NutcrackerState {
    WALKING,
    SENTRY
}

class NutcrackerController : IEnemyController<NutcrackerEnemyAI> {
    bool InSentryMode { get; set; } = false;

    public Vector3 GetCameraOffset(NutcrackerEnemyAI enemy) => new(0.0f, 2.8f, -3.0f);

    public void Update(NutcrackerEnemyAI enemy, bool isAIControlled) {
        if (isAIControlled) return;

        Reflector<NutcrackerEnemyAI> nutcracker = enemy.Reflect();
        float timeSinceFiringGun = nutcracker.GetInternalField<float>("timeSinceFiringGun");
        bool reloadingGun = nutcracker.GetInternalField<bool>("reloadingGun");
        bool aimingGun = nutcracker.GetInternalField<bool>("aimingGun");

        if (timeSinceFiringGun > 0.75f && enemy.gun.shellsLoaded <= 0 && !reloadingGun && !aimingGun) {
            enemy.ReloadGunServerRpc();
            enemy.SetBehaviourState(NutcrackerState.WALKING);
            this.InSentryMode = false;
        }

        if (this.InSentryMode) {
            return;
        }

        enemy.SwitchToBehaviourServerRpc(unchecked((int)NutcrackerState.WALKING));
    }

    public bool IsAbleToRotate(NutcrackerEnemyAI enemy) => !enemy.IsBehaviourState(NutcrackerState.SENTRY);

    public bool IsAbleToMove(NutcrackerEnemyAI enemy) => !enemy.IsBehaviourState(NutcrackerState.SENTRY);

    public void UsePrimarySkill(NutcrackerEnemyAI enemy) {
        bool reloadingGun = enemy.Reflect().GetInternalField<bool>("reloadingGun");

        if (enemy.gun is not ShotgunItem shotgun || enemy.gun.shellsLoaded <= 0 || reloadingGun) {
            return;
        }

        enemy.AimGunServerRpc(enemy.transform.forward);
        shotgun.gunShootAudio.volume = 0.25f;
        enemy.FireGunServerRpc();
    }

    public void OnSecondarySkillHold(NutcrackerEnemyAI enemy) {
        bool reloadingGun = enemy.Reflect().GetInternalField<bool>("reloadingGun");

        if (reloadingGun) {
            return;
        }

        enemy.SetBehaviourState(NutcrackerState.SENTRY);
        this.InSentryMode = true;
    }

    public void ReleaseSecondarySkill(NutcrackerEnemyAI enemy) {
        enemy.SetBehaviourState(NutcrackerState.WALKING);
        this.InSentryMode = false;
    }

    public void UseSpecialAbility(NutcrackerEnemyAI enemy) {
        Reflector<NutcrackerEnemyAI> nutcracker = enemy.Reflect();
        _ = nutcracker.GetInternalField<bool>("reloadingGun");
        int timesSeenSamePlayer = nutcracker.GetInternalField<int>("timesSeeingSamePlayer");
        int enemyHP = enemy.enemyHP;
        int shellsLoaded = enemy.gun.shellsLoaded;

        if (enemy.IsBehaviourState(NutcrackerState.WALKING)) {
            _ = nutcracker.SetInternalField("timesSeeingSamePlayer", 3);
            enemy.gun.shellsLoaded = 1;
            enemy.enemyHP = 1;
        }

        _ = nutcracker.SetInternalField("timesSeeingSamePlayer", timesSeenSamePlayer);
        enemy.AimGunServerRpc(enemy.transform.position);
        enemy.enemyHP = enemyHP;
        enemy.gun.shellsLoaded = shellsLoaded;
    }

    public void OnUnpossess(NutcrackerEnemyAI enemy) => this.InSentryMode = false;

    public string GetPrimarySkillName(NutcrackerEnemyAI enemy) => enemy.gun is null ? "" : "Fire";

    public string GetSecondarySkillName(NutcrackerEnemyAI _) => "(HOLD) Sentry mode";

    public void OnOutsideStatusChange(NutcrackerEnemyAI enemy) {
        enemy.StopSearch(enemy.attackSearch, true);
        enemy.StopSearch(enemy.patrol, true);
    }
}
