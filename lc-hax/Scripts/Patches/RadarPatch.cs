#pragma warning disable IDE1006

using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(ManualCameraRenderer), nameof(ManualCameraRenderer.SwitchRadarTargetClientRpc))]
class RadarPatch {
    static bool Prefix(ManualCameraRenderer __instance, int switchToIndex) {
        if (!Setting.EnableBlockRadar) return true;
        if (Helper.LocalPlayer?.playerClientId != (ulong)switchToIndex) return true;

        __instance.SwitchRadarTargetForward(true);
        return false;
    }
}
