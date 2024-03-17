using HarmonyLib;

[HarmonyPatch]
class HUDPatch {
    [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.HideHUD))]
    static void Prefix(ref bool hide) => hide = false;
}
