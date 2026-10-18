using System.Collections.Generic;
using HarmonyLib;
using UnityEngine.UI;

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.Start))]
sealed class SinkingOverlayPatch {
    static HashSet<Graphic> HiddenGraphics { get; } = [];

    static void Prefix() => SinkingOverlayPatch.HiddenGraphics.Clear();

    static void Postfix(HUDManager __instance) {
        if (__instance.sinkingUnderAnimator.Unfake() is not { } root) return;

        foreach (Graphic graphic in root.GetComponentsInChildren<Graphic>(includeInactive: true)) {
            _ = SinkingOverlayPatch.HiddenGraphics.Add(graphic);
            graphic.canvasRenderer.Clear();
        }

        root.gameObject.SetActive(false);
    }

    [HarmonyPatch(typeof(Graphic), nameof(Graphic.Rebuild))]
    [HarmonyPrefix]
    static bool PreventOverlayGeometry(Graphic __instance) {
        if (!SinkingOverlayPatch.HiddenGraphics.Contains(__instance)) return true;

        __instance.canvasRenderer.Clear();
        return false;
    }
}
