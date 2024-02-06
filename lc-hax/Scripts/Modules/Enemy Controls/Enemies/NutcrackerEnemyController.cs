public static class NutcrackerController {
    public static void UsePrimarySkill(this NutcrackerEnemyAI instance) {
        if (instance.gun is null) return;
        instance.FireGun();
    }

    public static void OnMoving(this NutcrackerEnemyAI instance, bool isMoving) {
        if(isMoving) instance.SetState(NutCrackerState.Walking);
    }

    public static bool IsReloadingGun(this NutcrackerEnemyAI instance) {
        return instance.Reflect().GetInternalField<bool>("reloadingGun");
    }

    public static void SetReloadingGunState(this NutcrackerEnemyAI instance, bool value) {
        _ = instance.Reflect().SetInternalField("reloadingGun", value);
    }

    public static void UseSecondarySkill(this NutcrackerEnemyAI instance) {
        instance.SetState(NutCrackerState.Sentry);
    }

    public static void ReleaseSecondarySkill(this NutcrackerEnemyAI instance) {
        instance.SetState(NutCrackerState.Walking);
    }

    public static string GetPrimarySkillName(this NutcrackerEnemyAI _) {
        return "Fire";
    }

    public static string GetSecondarySkillName(this NutcrackerEnemyAI _) {
        return "(HOLD) Sentry mode";
    }

    public static void FireGun(this NutcrackerEnemyAI instance) {
        instance.gun.gunShootAudio.volume = 0.25f;
        instance.FireGunServerRpc();
    }

    public static void ReloadGun(this NutcrackerEnemyAI instance) {
        instance.SetReloadingGunState(true);
        instance.ReloadGunServerRpc();
    }

    public static void SetState(this NutcrackerEnemyAI instance, NutCrackerState state) {
        if (instance.IsInState(state)) return;
        instance.SwitchToBehaviourServerRpc((int)state);
    }

    public static bool IsInState(this NutcrackerEnemyAI instance, NutCrackerState state) {
        return instance.currentBehaviourStateIndex == (int)state;
    }

    public enum NutCrackerState {
        Walking = 0,
        Sentry = 1
    }
}
