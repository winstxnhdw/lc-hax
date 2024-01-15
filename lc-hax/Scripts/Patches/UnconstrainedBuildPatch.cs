#pragma warning disable IDE1006

using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(ShipBuildModeManager), "PlayerMeetsConditionsToBuild")]
class UnconstrainedBuildPatch {
    static bool Prefix(ref bool __result, ref bool ___CanConfirmPosition, ref PlaceableShipObject? ___placingObject) {
        if (!___placingObject.IsNotNull(out PlaceableShipObject placingObject)) return true;

        placingObject.AllowPlacementOnCounters = true;
        placingObject.AllowPlacementOnWalls = true;
        ___CanConfirmPosition = true;
        __result = Helper.LocalPlayer?.inTerminalMenu is false;

        return false;
    }
}
