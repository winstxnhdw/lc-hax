#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.AllowPlayerDeath))]
class GodModePatch {
    static bool Prefix(PlayerControllerB __instance, ref bool __result) {
        if (!Setting.EnableGodMode) return true;
        if (!__instance.IsSelf()) return true;

        __result = false;
        __instance.inAnimationWithEnemy = null;
        __instance.inSpecialInteractAnimation = false;

        return false;
    }
}
