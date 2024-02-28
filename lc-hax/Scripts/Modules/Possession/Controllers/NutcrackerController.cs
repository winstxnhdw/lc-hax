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

        float timeSinceFiringGun = enemy.Reflect().GetInternalField<float>("timeSinceFiringGun");
        bool reloadingGun = enemy.Reflect().GetInternalField<bool>("reloadingGun");
        bool aimingGun = enemy.Reflect().GetInternalField<bool>("aimingGun");

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

    public void UseSpecialAbility(NutcrackerEnemyAI enemy) => enemy.ReloadGunServerRpc();

    public void OnUnpossess(NutcrackerEnemyAI enemy) => this.InSentryMode = false;

    public string GetPrimarySkillName(NutcrackerEnemyAI enemy) => enemy.gun is null ? "" : "Fire";

    public string GetSecondarySkillName(NutcrackerEnemyAI _) => "(HOLD) Sentry mode";

    public float InteractRange(NutcrackerEnemyAI _) => 1.5f;

    public void OnOutsideStatusChange(NutcrackerEnemyAI enemy) {
        enemy.StopSearch(enemy.attackSearch, true);
        enemy.StopSearch(enemy.patrol, true);
    }

}
