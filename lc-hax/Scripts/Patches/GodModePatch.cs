using GameNetcodeStuff;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(PlayerControllerB))]
[HarmonyPatch("AllowPlayerDeath")]
class GodModePatch {
    static bool Prefix() => !Settings.EnableGodMode;
}
