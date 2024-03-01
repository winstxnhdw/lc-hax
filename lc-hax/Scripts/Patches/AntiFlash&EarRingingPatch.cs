using HarmonyLib;

class AntiFlashAndEarRingingPatch {
    [HarmonyPatch(typeof(HUDManager), "Update")]
    class AntiFlashPatch {
        [HarmonyPrefix]
        static bool Prefix(HUDManager __instance) {
            __instance.flashFilter = 0f;
            return true;
        }
    }

    [HarmonyPatch(typeof(SoundManager), "SetEarsRinging")]
    class AntiEarRingingPatch {
        [HarmonyPrefix]
        static bool Prefix() {
            return false;
        }
    }
}
