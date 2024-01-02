using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DamagePlayer))]
class DemigodModePatch {
    static bool Prefix() => !Setting.EnableDemigodMode;
}
