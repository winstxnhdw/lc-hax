using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.AllowPlayerDeath))]
class GodModePatch {
    static bool Prefix() => !Setting.EnableGodMode;
}
