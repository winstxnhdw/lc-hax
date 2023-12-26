using GameNetcodeStuff;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DamagePlayer))]
class DemigodModePatch {
    static bool Prefix() => !Settings.EnableDemigodMode;
}
