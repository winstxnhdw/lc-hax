using HarmonyLib;

[HarmonyPatch(typeof(ShipBuildModeManager), nameof(ShipBuildModeManager.PlayerMeetsConditionsToBuild))]
sealed class UnconstrainedBuildPatch {
    static bool Prefix(ref bool __result, ref bool ___CanConfirmPosition, ref PlaceableShipObject? ___placingObject) {
        if (___placingObject.Unfake() is not PlaceableShipObject placingObject) return true;

        placingObject.AllowPlacementOnCounters = true;
        placingObject.AllowPlacementOnWalls = true;
        ___CanConfirmPosition = true;
        __result = Helper.LocalPlayer is { inTerminalMenu: false };

        return false;
    }
}
