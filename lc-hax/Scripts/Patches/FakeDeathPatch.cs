using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(PlayerControllerB), "KillPlayerClientRpc")]
class FakeDeathPatch {
    static bool Prefix(int playerId) {
        if (!Setting.EnableFakeDeath) return true;

        Setting.EnableFakeDeath = false;
        return Helper.LocalPlayer?.ClientId() != playerId;
    }
}
