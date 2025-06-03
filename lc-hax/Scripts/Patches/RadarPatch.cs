#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(ManualCameraRenderer), nameof(ManualCameraRenderer.SwitchRadarTargetClientRpc))]
class RadarPatch {
    static bool Prefix(ManualCameraRenderer __instance, int switchToIndex) {
        if (!Setting.EnableBlockRadar) return true;
        if (Helper.LocalPlayer?.PlayerIndex() != switchToIndex) return true;

        __instance.SwitchRadarTargetForward(true);
        return false;
    }
}
