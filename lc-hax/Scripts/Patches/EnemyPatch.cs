#pragma warning disable IDE1006

#region

using HarmonyLib;

#endregion

[HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.SyncPositionToClients))]
class EnemyPatch {
    static void Prefix(EnemyAI __instance) => __instance.updatePositionThreshold = 0.0f;
}
