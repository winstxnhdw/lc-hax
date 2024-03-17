#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch]
class PossessionPatch {
    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.VoteShipToLeaveEarly))]
    static bool Prefix() => PossessionMod.Instance is not { IsPossessed: true };

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HUDManager), "PingScan_performed")]
    static bool PingScanPrefix() =>
        HaxCamera.Instance?.HaxCameraContainer is not { activeSelf: true } &&
        PossessionMod.Instance is not { IsPossessed: true };

    [HarmonyPatch(typeof(HUDManager), "Update")]
    static void Postfix(HUDManager __instance, ref float ___holdButtonToEndGameEarlyHoldTime) {
        if (PossessionMod.Instance is not { IsPossessed: true }) return;

        ___holdButtonToEndGameEarlyHoldTime = 0.0f;
        __instance.holdButtonToEndGameEarlyMeter?.gameObject.SetActive(false);
    }

    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.PlayerIsTargetable))]
    static bool Prefix(
        EnemyAI __instance,
        PlayerControllerB playerScript,
        ref bool cannotBeInShip,
        ref bool overrideInsideFactoryCheck,
        ref bool __result
    ) {
        if (PossessionMod.Instance is { IsPossessed: true } && PossessionMod.Instance.PossessedEnemy == __instance) {
            if (playerScript.IsDead()) return true;

            __result = true;
            return false;
        }

        if (__instance.isOutside != __instance.enemyType.isOutsideEnemy) {
            overrideInsideFactoryCheck = true;
            cannotBeInShip = false;
        }

        return true;
    }
}
