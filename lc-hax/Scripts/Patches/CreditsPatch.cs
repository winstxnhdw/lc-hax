#pragma warning disable IDE1006

using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(Terminal), nameof(Terminal.SyncGroupCreditsClientRpc))]
class CreditsPatch {
    static bool IsSynced { get; set; } = false;

    static bool Prefix(Terminal __instance) {
        if (!Setting.EnableBlockCredits || CreditsPatch.IsSynced) return true;

        CreditsPatch.IsSynced = true;

        __instance.SyncGroupCreditsServerRpc(
            __instance.groupCredits,
            __instance.numberOfItemsInDropship
        );

        CreditsPatch.IsSynced = false;

        return false;
    }
}
