#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.AllowPlayerDeath))]
class GodModePatch {
    static bool Prefix(ulong ___actualClientId) => !Setting.EnableGodMode || Helper.LocalPlayer?.actualClientId != ___actualClientId;
}
