using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB), "KillPlayerClientRpc")]
sealed class FakeDeathPatch {
    static bool Prefix(int playerId) {
        if (!Setting.EnableFakeDeath) return true;

        Setting.EnableFakeDeath = false;
        return Helper.LocalPlayer?.PlayerIndex() != playerId;
    }
}
