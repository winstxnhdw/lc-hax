using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(PlayerControllerB), "KillPlayerClientRpc")]
internal class FakeDeathPatch
{
    private static bool Prefix(int playerId)
    {
        if (!Setting.EnableFakeDeath) return true;

        Setting.EnableFakeDeath = false;
        return Helper.LocalPlayer?.PlayerIndex() != playerId;
    }
}