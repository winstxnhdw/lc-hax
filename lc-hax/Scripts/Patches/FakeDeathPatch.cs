using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(PlayerControllerB), "KillPlayerClientRpc")]
class FakeDeathPatch {
    static bool Prefix(ref int playerId) {
        if (!Setting.EnableFakeDeath) return false;
        Setting.EnableFakeDeath = false;

        return Helper.LocalPlayer?.actualClientId != (ulong)playerId;
    }
}
