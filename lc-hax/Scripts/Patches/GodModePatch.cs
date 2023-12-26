using GameNetcodeStuff;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.AllowPlayerDeath))]
class GodModePatch {
    static bool Prefix() => !Settings.EnableGodMode;
}
