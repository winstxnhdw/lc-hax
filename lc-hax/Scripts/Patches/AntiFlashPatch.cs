#pragma warning disable IDE1006

#region

using HarmonyLib;

#endregion

class AntiFlashPatch {
    [HarmonyPatch(typeof(HUDManager), "Update")]
    static void Prefix(HUDManager __instance) => __instance.flashFilter = 0.0f;

    [HarmonyPatch(typeof(SoundManager), "SetEarsRinging")]
    static bool Prefix() => false;
}
