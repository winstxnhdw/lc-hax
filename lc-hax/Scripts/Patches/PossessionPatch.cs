using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

[HarmonyPatch]
sealed class PossessionPatch {
    internal static Possession? GetPossession(Component? target) => target.Unfake()?.GetComponent<PossessionBinding>()?.Possession;

    [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.Update))]
    static void Postfix(HUDManager __instance, ref float ___holdButtonToEndGameEarlyHoldTime) {
        if (PossessionPatch.GetPossession(Helper.LocalPlayer) is not { IsPossessed: true }) return;

        ___holdButtonToEndGameEarlyHoldTime = 0.0f;
        __instance.holdButtonToEndGameEarlyMeter?.gameObject.SetActive(false);
    }

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.VoteShipToLeaveEarly))]
    static bool Prefix() => PossessionPatch.GetPossession(Helper.LocalPlayer) is not { IsPossessed: true };

    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.ChangeOwnershipOfEnemy))]
    static bool Prefix(EnemyAI __instance, ulong newOwnerClientId) =>
        PossessionPatch.GetPossession(__instance) is not Possession possession || possession.PossessedEnemy != __instance || possession.IsAIControlled || Helper.LocalPlayer?.actualClientId == newOwnerClientId;

    [HarmonyPatch(typeof(SandSpiderAI), nameof(SandSpiderAI.SyncMeshContainerPositionToClients))]
    static bool Prefix(SandSpiderAI __instance) => PossessionPatch.GetPossession(__instance)?.IsManuallyControlled(__instance) is not true;

    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.PlayerIsTargetable))]
    static void Prefix(EnemyAI __instance, ref bool overrideInsideFactoryCheck, ref bool checkForMineshaftStartTile) {
        if (PossessionPatch.GetPossession(__instance) is not { IsPossessed: true }) return;

        overrideInsideFactoryCheck = true;
        checkForMineshaftStartTile = false;
    }

    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.OnCollideWithPlayer))]
    static void Postfix(EnemyAI __instance, Collider other) {
        if (PossessionPatch.GetPossession(__instance) is not Possession possession) return;
        possession.OnPlayerCollision(other);
    }
}

[HarmonyPatch(typeof(SandWormAI), nameof(SandWormAI.StartEmergeAnimation))]
sealed class SandWormPossessionPatch {
    [HarmonyPostfix]
    [HarmonyInfix(typeof(NetworkBehaviour), nameof(NetworkBehaviour.IsServer), InnerTargetKind.Getter)]
    static void AllowManualEmerge([HarmonyOuter] SandWormAI __instance, ref bool __result) =>
        __result = __result || PossessionPatch.GetPossession(__instance)?.IsManuallyControlled(__instance) is true;
}

[HarmonyPatch(typeof(StingrayAI), nameof(StingrayAI.Update))]
sealed class StingrayPossessionPatch {
    // Later owner checks must still run for slime creation and aiming.
    [HarmonyPostfix]
    [HarmonyInfix(typeof(NetworkBehaviour), nameof(NetworkBehaviour.IsOwner), InnerTargetKind.Getter, Positions = new[] { 1 })]
    static void KeepManualOwnership([HarmonyOuter] StingrayAI __instance, ref bool __result) =>
        __result = __result && PossessionPatch.GetPossession(__instance)?.IsManuallyControlled(__instance) is not true;
}
