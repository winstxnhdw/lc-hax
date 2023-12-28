using GameNetcodeStuff;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(PlayerControllerB), "DamagePlayer")]
class NoFallDamagePatch {
    static bool Prefix(CauseOfDeath causeOfDeath) => !Setting.DisableFallDamage || causeOfDeath != CauseOfDeath.Gravity;
}
