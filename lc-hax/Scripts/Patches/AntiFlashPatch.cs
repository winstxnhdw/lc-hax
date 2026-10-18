using HarmonyLib;

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.Update))]
sealed class AntiFlashPatch {
    static void Prefix(HUDManager __instance) => __instance.flashFilter = 0.0f;
}
