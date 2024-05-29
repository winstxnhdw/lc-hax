using HarmonyLib;

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.HideHUD))]
internal class HUDPatch
{
    private static void Prefix(ref bool hide)
    {
        hide = false;
    }
}