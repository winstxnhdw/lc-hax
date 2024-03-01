using HarmonyLib;

class AntiFlashAndEarRingingPatch {
    [HarmonyPatch(typeof(HUDManager), "Update")]
    class AntiFlashPatch {
        [HarmonyPrefix]
        static void Prefix(HUDManager __instance) => __instance.flashFilter = 0f;
    }

    [HarmonyPatch(typeof(SoundManager), "SetEarsRinging")]
    class AntiEarRingingPatch {
        [HarmonyPrefix]
        static bool Prefix() => false;
    }
}
