using HarmonyLib;

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.Start))]
sealed class SinkingOverlayPatch {
    static void Postfix(HUDManager __instance) => ScreenOverlayPatch.Hide(__instance.sinkingUnderAnimator);
}
