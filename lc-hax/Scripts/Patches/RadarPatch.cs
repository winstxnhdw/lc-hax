using HarmonyLib;

[HarmonyPatch(typeof(ManualCameraRenderer), nameof(ManualCameraRenderer.SwitchRadarTargetClientRpc))]
sealed class RadarPatch {
    static bool Prefix(ManualCameraRenderer __instance, int switchToIndex) {
        if (!Globals.EnableBlockRadar) return true;
        if (Helper.LocalPlayer?.PlayerIndex() != switchToIndex) return true;

        __instance.SwitchRadarTargetForward(true);
        return false;
    }
}
