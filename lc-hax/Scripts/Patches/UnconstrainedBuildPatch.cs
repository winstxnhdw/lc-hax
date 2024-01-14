#pragma warning disable IDE1006

using HarmonyLib;
using UnityEngine;
using Hax;

[HarmonyPatch(typeof(ShipBuildModeManager), "PlayerMeetsConditionsToBuild")]
class UnconstrainedBuildPatch {
    static bool Prefix(ref bool __result, ref bool ___CanConfirmPosition, ref PlaceableShipObject ___placingObject) {
        ___CanConfirmPosition = true;
        ___placingObject.AllowPlacementOnCounters = true;
        ___placingObject.AllowPlacementOnWalls = true;
        __result = Helper.LocalPlayer?.inTerminalMenu is false;

        return false;
    }
}
