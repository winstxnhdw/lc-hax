using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.SetPlayerSanityLevel))]
sealed class SanePatch {
    static bool Prefix(PlayerControllerB __instance) {
        if (!__instance.IsSelf()) return true;

        __instance.insanityLevel = 0.0f;
        __instance.insanitySpeedMultiplier = 0.0f;

        return false;
    }
}
