#pragma warning disable IDE1006

#region

using GameNetcodeStuff;
using HarmonyLib;
using Hax;

#endregion

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.AllowPlayerDeath))]
class GodModePatch {
    static bool Prefix(PlayerControllerB __instance, ref bool __result) {
        if (!Setting.EnableGodMode || !__instance.IsSelf()) return true;

        __result = false;
        __instance.inAnimationWithEnemy = null;
        __instance.inSpecialInteractAnimation = false;
        __instance.health = 100;
        if (Helper.HUDManager is HUDManager hudManager) {
            hudManager.localPlayer.playerBodyAnimator.SetBool("Limp", false);
            hudManager.HUDAnimator.SetBool("biohazardDamage", false);
            hudManager.HUDAnimator.SetTrigger("HealFromCritical");
            hudManager.UpdateHealthUI(hudManager.localPlayer.health, false);
        }

        return false;
    }
}
