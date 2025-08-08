#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.SyncPositionToClients))]
sealed class EnemyPatch {
    static void Prefix(EnemyAI __instance) => __instance.updatePositionThreshold = 0.0f;
}
