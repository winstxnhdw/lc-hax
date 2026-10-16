using HarmonyLib;

sealed class AntiFlashPatch {
    [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.Update))]
    static void Prefix(HUDManager __instance) => __instance.flashFilter = 0.0f;

    [HarmonyPatch(typeof(SoundManager), nameof(SoundManager.SetEarsRinging))]
    static bool Prefix() => false;
}
