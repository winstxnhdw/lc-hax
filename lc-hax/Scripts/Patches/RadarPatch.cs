#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(ManualCameraRenderer), nameof(ManualCameraRenderer.SwitchRadarTargetClientRpc))]
class RadarPatch {
    static bool Prefix(ManualCameraRenderer __instance, int switchToIndex) {
        if (!Setting.EnableBlockRadar) return true;
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) return true;
        if (switchToIndex != (int)localPlayer.playerClientId) return true;

        __instance.SwitchRadarTargetForward(true);
        return false;
    }
}
