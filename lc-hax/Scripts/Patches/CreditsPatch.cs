#pragma warning disable IDE1006

using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(Terminal), nameof(Terminal.SyncGroupCreditsClientRpc))]
class CreditsPatch {
    static bool IsNotSynced { get; set; } = true;

    static bool Prefix(Terminal __instance) {
        if (!Settings.EnableBlockCredits) return true;
        if (CreditsPatch.IsNotSynced) {
            __instance.SyncGroupCreditsServerRpc(
                __instance.groupCredits,
                __instance.numberOfItemsInDropship
            );
        }

        CreditsPatch.IsNotSynced = !CreditsPatch.IsNotSynced;
        return false;
    }
}
