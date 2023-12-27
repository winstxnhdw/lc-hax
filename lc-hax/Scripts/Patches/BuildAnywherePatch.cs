#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(ShipBuildModeManager))]
[HarmonyPatch("PlayerMeetsConditionsToBuild")]
class BuildAnywherePatch {
    static bool Prefix(ref bool __result, ref bool ___CanConfirmPosition) {
        ___CanConfirmPosition = true;
        __result = !Helper.LocalPlayer.IsNotNull(out PlayerControllerB player) || !player.inTerminalMenu;
        return false;
    }
}
