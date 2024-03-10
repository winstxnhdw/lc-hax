#pragma warning disable IDE1006
using HarmonyLib;
using GameNetcodeStuff;
using Hax;

[HarmonyPatch(typeof(PlayerControllerB), "CheckConditionsForSinkingInQuicksand")]
class NoSinking {
    public static bool Prefix(PlayerControllerB __instance, ref bool __result) {
        if(!Setting.EnableNoSinking) return true;
        if(__instance.IsSelf()) {
            __result = false;
            return false;
        }
        return true;
    }
}
