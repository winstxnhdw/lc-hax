using Hax;
using UnityEngine;

enum NutcrackerState {
    WALKING,
    SENTRY
}

internal class NutcrackerController : IEnemyController<NutcrackerEnemyAI> {

    Vector3 CamOffset = new(0, 2.8f, -3f);

    public Vector3 GetCameraOffset(NutcrackerEnemyAI enemy) => this.CamOffset;

    bool InSentryMode { get; set; } = false;

    public void Update(NutcrackerEnemyAI enemy, bool isAIControlled) {
        if (isAIControlled) return;
        Reflector<NutcrackerEnemyAI> Nutcracker = enemy.Reflect();

        float timeSinceFiringGun = Nutcracker.GetInternalField<float>("timeSinceFiringGun");
        bool reloadingGun = Nutcracker.GetInternalField<bool>("reloadingGun");
        bool aimingGun = Nutcracker.GetInternalField<bool>("aimingGun");

        if (timeSinceFiringGun > 0.75f && enemy.gun.shellsLoaded <= 0 && !reloadingGun && !aimingGun) {
            enemy.ReloadGunServerRpc();
            enemy.SetBehaviourState(NutcrackerState.WALKING);
            this.InSentryMode = false;
        }

        if (this.InSentryMode) return;
        enemy.SwitchToBehaviourServerRpc(unchecked((int)NutcrackerState.WALKING));
    }

    public bool IsAbleToRotate(NutcrackerEnemyAI enemy) => !enemy.IsBehaviourState(NutcrackerState.SENTRY);

    public bool IsAbleToMove(NutcrackerEnemyAI enemy) => !enemy.IsBehaviourState(NutcrackerState.SENTRY);

    public void UsePrimarySkill(NutcrackerEnemyAI enemy) {
        bool reloadingGun = enemy.Reflect().GetInternalField<bool>("reloadingGun");
        if (enemy.gun is not ShotgunItem shotgun || enemy.gun.shellsLoaded <= 0 || reloadingGun ) return;
        enemy.AimGunServerRpc(enemy.transform.forward);
        shotgun.gunShootAudio.volume = 0.25f;
        enemy.FireGunServerRpc();
    }

    public void OnSecondarySkillHold(NutcrackerEnemyAI enemy) {
        bool reloadingGun = enemy.Reflect().GetInternalField<bool>("reloadingGun");
        if (reloadingGun) return;
        enemy.SetBehaviourState(NutcrackerState.SENTRY);
        this.InSentryMode = true;
    }

    public void ReleaseSecondarySkill(NutcrackerEnemyAI enemy) {
        enemy.SetBehaviourState(NutcrackerState.WALKING);
        this.InSentryMode = false;
    }

    public void UseSpecialAbility(NutcrackerEnemyAI enemy) {
        bool reloadingGun = enemy.Reflect().GetInternalField<bool>("reloadingGun");
        Reflector<NutcrackerEnemyAI> Nutcracker = enemy.Reflect();
        int SaveTimesSeeingSamePlayer = Nutcracker.GetInternalField<int>("timesSeeingSamePlayer");
        int SaveHP = enemy.enemyHP;
        int SaveShellsLoaded = enemy.gun.shellsLoaded;
        if (enemy.IsBehaviourState(NutcrackerState.WALKING)) {
            _ = Nutcracker.SetInternalField("timesSeeingSamePlayer", 3);
            enemy.gun.shellsLoaded = 1;
            enemy.enemyHP = 1;
        }
        enemy.AimGunServerRpc(enemy.transform.position);
        _ = Nutcracker.SetInternalField("timesSeeingSamePlayer", SaveTimesSeeingSamePlayer);
        enemy.enemyHP = SaveHP;
        enemy.gun.shellsLoaded = SaveShellsLoaded;
    }
    
    public void OnUnpossess(NutcrackerEnemyAI enemy) => this.InSentryMode = false;

    public string GetPrimarySkillName(NutcrackerEnemyAI enemy) => enemy.gun is null ? "" : "Fire";

    public string GetSecondarySkillName(NutcrackerEnemyAI _) => "(HOLD) Sentry mode";

    public void OnOutsideStatusChange(NutcrackerEnemyAI enemy) {
        enemy.StopSearch(enemy.attackSearch, true);
        enemy.StopSearch(enemy.patrol, true);
    }

}
