#pragma warning disable IDE1006

#region

using GameNetcodeStuff;
using HarmonyLib;
using Hax;

#endregion

[HarmonyPatch(typeof(PlayerControllerB), "CheckConditionsForSinkingInQuicksand")]
class NoSinking {
    public static bool Prefix(PlayerControllerB __instance, ref bool __result) {
        if (!Setting.EnableNoSinking) return true;
        if (__instance.IsSelf()) {
            __result = false;
            return false;
        }

        return true;
    }
}
