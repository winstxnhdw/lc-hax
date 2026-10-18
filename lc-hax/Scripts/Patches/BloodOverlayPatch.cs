using System.Collections.Generic;
using HarmonyLib;
using UnityEngine.UI;

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.Start))]
sealed class BloodOverlayPatch {
    static HashSet<Graphic> HiddenGraphics { get; } = [];

    static void Prefix() => BloodOverlayPatch.HiddenGraphics.Clear();

    static void Postfix(HUDManager __instance) {
        if (__instance.HUDContainer.Unfake()?.transform.Find("SpecialHUDGraphics/BloodTex").Unfake() is not { } root) return;

        foreach (Graphic graphic in root.GetComponentsInChildren<Graphic>(includeInactive: true)) {
            _ = BloodOverlayPatch.HiddenGraphics.Add(graphic);
            graphic.canvasRenderer.Clear();
        }

        root.gameObject.SetActive(false);
    }

    [HarmonyPatch(typeof(Graphic), nameof(Graphic.Rebuild))]
    [HarmonyPrefix]
    static bool PreventOverlayGeometry(Graphic __instance) {
        if (!BloodOverlayPatch.HiddenGraphics.Contains(__instance)) return true;

        __instance.canvasRenderer.Clear();
        return false;
    }
}
