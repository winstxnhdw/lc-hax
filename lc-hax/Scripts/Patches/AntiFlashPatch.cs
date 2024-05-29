#pragma warning disable IDE1006

using HarmonyLib;

internal class AntiFlashPatch
{
    [HarmonyPatch(typeof(HUDManager), "Update")]
    private static void Prefix(HUDManager __instance)
    {
        __instance.flashFilter = 0.0f;
    }

    [HarmonyPatch(typeof(SoundManager), "SetEarsRinging")]
    private static bool Prefix()
    {
        return false;
    }
}