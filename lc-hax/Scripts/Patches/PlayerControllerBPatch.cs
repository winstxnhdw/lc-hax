using GameNetcodeStuff;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(PlayerControllerB))]
[HarmonyPatch("AllowPlayerDeath")]
public class PlayerControllerBPatch {
    static bool Prefix() => !Settings.EnableGodMode;
}
