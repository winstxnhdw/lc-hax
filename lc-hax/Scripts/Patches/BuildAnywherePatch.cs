#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(ShipBuildModeManager), "PlayerMeetsConditionsToBuild")]
class BuildAnywherePatch {
    static bool Prefix(ref bool __result, ref bool ___CanConfirmPosition) {
        ___CanConfirmPosition = true;
        __result = !Helper.LocalPlayer.IsNotNull(out PlayerControllerB player) || !player.inTerminalMenu;
        return false;
    }
}
