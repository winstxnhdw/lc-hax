using GameNetcodeStuff;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(PlayerControllerB))]
[HarmonyPatch("DamagePlayer")]
class NoFallDamagePatch {
    static bool Prefix(ref CauseOfDeath causeOfDeath) {
        return !Settings.DisableFallDamage || causeOfDeath != CauseOfDeath.Gravity;
    }
}
