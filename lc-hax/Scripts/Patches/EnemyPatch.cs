#pragma warning disable IDE1006

using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.SyncPositionToClients))]
class EnemyPatch {
    static void Prefix(ref float ___updatePositionThreshold) => ___updatePositionThreshold = 0.0f;
}
