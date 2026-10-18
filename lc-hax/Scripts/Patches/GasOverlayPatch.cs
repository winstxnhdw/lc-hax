using HarmonyLib;

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.Start))]
sealed class GasOverlayPatch {
    static void Postfix(HUDManager __instance) => ScreenOverlayPatch.Hide(__instance.gasImageAlpha);
}
