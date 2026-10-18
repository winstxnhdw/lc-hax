using HarmonyLib;

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.Start))]
sealed class BloodOverlayPatch {
    static void Postfix(HUDManager __instance) =>
        ScreenOverlayPatch.Hide(__instance.HUDContainer.Unfake()?.transform.Find("SpecialHUDGraphics/BloodTex"));
}
