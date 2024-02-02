public static class NutcrackerController {


    public static void UsePrimarySkill(this NutcrackerEnemyAI instance) {
        if (instance.gun != null) {
            instance.FireGun();
        }
    }

    public static void OnMoving(this NutcrackerEnemyAI instance) {
        if (instance == null) return;
        instance.SetState(NutCrackerState.Walking);
    }


    public static bool Get_IsReloadingGun(this NutcrackerEnemyAI instance) {
        return instance.Reflect().GetInternalField<bool>("reloadingGun");
    }
    public static void Set_IsReloadingGun(this NutcrackerEnemyAI instance, bool value) {
        _ = instance.Reflect().SetInternalField("reloadingGun", value);
    }

    public static void UseSecondarySkill(this NutcrackerEnemyAI instance) {
        if (instance == null) return;
        instance.SetState(NutCrackerState.Sentry);
    }

    public static void ReleaseSecondarySkill(this NutcrackerEnemyAI instance) {
        if (instance == null) return;
        instance.SetState(NutCrackerState.Walking);
    }

    public static string GetPrimarySkillName(this NutcrackerEnemyAI instance) {
        return "Fire";
    }

    public static string GetSecondarySkillName(this NutcrackerEnemyAI instance) {
        return "(HOLD) Sentry mode";
    }

    public static void FireGun(this NutcrackerEnemyAI instance) {
        instance.gun.gunShootAudio.volume = 0.25f;
        instance.FireGunServerRpc();
    }

    public static void ReloadGun(this NutcrackerEnemyAI instance) {
        instance.Set_IsReloadingGun(true);
        instance.ReloadGunServerRpc();
    }

    public static void SetState(this NutcrackerEnemyAI instance, NutCrackerState state) {
        if (instance == null) return;
        if (!instance.IsInState(state))
            instance.SwitchToBehaviourServerRpc((int)state);
    }

    public static bool IsInState(this NutcrackerEnemyAI instance, NutCrackerState state) {
        return instance != null && instance.currentBehaviourStateIndex == (int)state;
    }

    public enum NutCrackerState {
        Walking = 0,
        Sentry = 1
    }
}
