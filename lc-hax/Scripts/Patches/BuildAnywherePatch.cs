#pragma warning disable IDE1006

using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(ShipBuildModeManager))]
[HarmonyPatch("PlayerMeetsConditionsToBuild")]
class BuildAnywherePatch {
    static bool Prefix(ref bool __result) {
        __result = true;
        return false;
    }
}
