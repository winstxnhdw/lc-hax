using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.HideHUD))]
class HUDPatch {
    static void Prefix(ref bool hide) => hide = false;
}
