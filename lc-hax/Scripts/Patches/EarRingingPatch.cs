using HarmonyLib;

[HarmonyPatch(typeof(SoundManager), nameof(SoundManager.SetEarsRinging))]
sealed class EarRingingPatch {
    static bool Prefix() => false;
}
