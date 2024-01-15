using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(PlayerControllerB), "KillPlayerClientRpc")]
class FakeDeathPatch {
    static bool Prefix(ref int playerId) => !Setting.EnableFakeDeath || Helper.LocalPlayer?.actualClientId != (ulong)playerId;
}
