using Unity.Netcode;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(RoundManager), nameof(RoundManager.SyncScrapValuesClientRpc))]
class ScrapValuesPatch {
    static bool Prefix(NetworkObjectReference[] spawnedScrap, ref int[] allScrapValue) {
        if (Settings.ScrapValue is -1 || spawnedScrap == null) return true;

        for (int i = 0; i < spawnedScrap.Length; i++) {
            if (!spawnedScrap[i].TryGet(out NetworkObject networkObject)) continue;
            if (networkObject.GetComponent<GrabbableObject>() == null) continue;

            allScrapValue[i] = Settings.ScrapValue;
        }

        return true;
    }
}
