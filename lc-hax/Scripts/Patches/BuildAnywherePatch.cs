#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(ShipBuildModeManager))]
[HarmonyPatch("PlayerMeetsConditionsToBuild")]
class BuildAnywherePatch {
    static bool Prefix(ref bool __result) {
        __result = !Helper.LocalPlayer.IsNotNull(out PlayerControllerB player) || !player.inTerminalMenu;
        return false;
    }
}
