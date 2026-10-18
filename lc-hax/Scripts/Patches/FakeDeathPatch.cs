using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.KillPlayerClientRpc))]
sealed class FakeDeathPatch {
    static bool Prefix(int playerId) {
        if (!Globals.EnableFakeDeath) return true;

        Globals.EnableFakeDeath = false;
        return Helper.LocalPlayer?.PlayerIndex() != playerId;
    }
}
