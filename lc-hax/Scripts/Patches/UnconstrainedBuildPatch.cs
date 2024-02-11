#pragma warning disable IDE1006

using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(ShipBuildModeManager), "PlayerMeetsConditionsToBuild")]
class UnconstrainedBuildPatch {
    static bool Prefix(ref bool __result, ref bool ___CanConfirmPosition, ref PlaceableShipObject? ___placingObject) {
        if (___placingObject is null) return true;

        ___placingObject.AllowPlacementOnCounters = true;
        ___placingObject.AllowPlacementOnWalls = true;
        ___CanConfirmPosition = true;
        __result = Helper.LocalPlayer?.inTerminalMenu is not true;

        return false;
    }
}
