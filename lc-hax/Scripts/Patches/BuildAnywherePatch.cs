#pragma warning disable IDE1006

using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(ShipBuildModeManager), "PlayerMeetsConditionsToBuild")]
class BuildAnywherePatch {
    static bool Prefix(ref bool __result, ref bool ___CanConfirmPosition) {
        ___CanConfirmPosition = true;
        __result = Helper.LocalPlayer?.inTerminalMenu is false;

        return false;
    }
}
