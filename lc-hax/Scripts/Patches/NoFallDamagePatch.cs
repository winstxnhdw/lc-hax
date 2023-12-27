using GameNetcodeStuff;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(PlayerControllerB))]
[HarmonyPatch("DamagePlayer")]
class NoFallDamagePatch {
    static bool Prefix(CauseOfDeath causeOfDeath) => !Setting.DisableFallDamage || causeOfDeath != CauseOfDeath.Gravity;
}
