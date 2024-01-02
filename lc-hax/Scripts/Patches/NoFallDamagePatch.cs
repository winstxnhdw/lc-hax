using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(PlayerControllerB), "DamagePlayer")]
class NoFallDamagePatch {
    static bool Prefix(CauseOfDeath causeOfDeath) => !Setting.DisableFallDamage || causeOfDeath != CauseOfDeath.Gravity;
}
