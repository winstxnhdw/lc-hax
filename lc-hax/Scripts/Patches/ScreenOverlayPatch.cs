using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.Start))]
sealed class ScreenOverlayPatch {
    static HashSet<Graphic> HiddenGraphics { get; } = [];

    static void Prefix() => ScreenOverlayPatch.HiddenGraphics.Clear();

    [HarmonyPatch(typeof(Graphic), nameof(Graphic.Rebuild))]
    [HarmonyPrefix]
    static bool PreventOverlayGeometry(Graphic __instance) {
        if (!ScreenOverlayPatch.HiddenGraphics.Contains(__instance)) return true;

        __instance.canvasRenderer.Clear();
        return false;
    }

    internal static void Hide(Component? root) {
        if (root.Unfake() is not { } component) return;

        foreach (Graphic graphic in component.GetComponentsInChildren<Graphic>(includeInactive: true)) {
            _ = ScreenOverlayPatch.HiddenGraphics.Add(graphic);
            graphic.canvasRenderer.Clear();
        }

        component.gameObject.SetActive(false);
    }
}
