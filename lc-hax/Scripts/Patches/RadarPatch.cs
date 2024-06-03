#pragma warning disable IDE1006

#region

using HarmonyLib;
using Hax;

#endregion

[HarmonyPatch(typeof(ManualCameraRenderer), nameof(ManualCameraRenderer.SwitchRadarTargetClientRpc))]
class RadarPatch {
    static bool Prefix(ManualCameraRenderer __instance, int switchToIndex) {
        if (!Setting.EnableBlockRadar || Helper.LocalPlayer?.GetPlayerId() != switchToIndex) return true;

        __instance.SwitchRadarTargetForward(true);
        return false;
    }
}
