#pragma warning disable IDE1006

using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(Terminal), nameof(Terminal.SyncGroupCreditsClientRpc))]
class CreditsPatch {
    static bool IsSynced { get; set; } = false;

    static bool Prefix(Terminal __instance, int newGroupCredits) {
        if (!Setting.EnableBlockCredits || CreditsPatch.IsSynced) return true;
        if (newGroupCredits == __instance.groupCredits) return true;

        CreditsPatch.IsSynced = true;

        __instance.SyncGroupCreditsServerRpc(
            __instance.groupCredits,
            __instance.numberOfItemsInDropship
        );

        CreditsPatch.IsSynced = false;
        Console.Print("A player has attempted to modify the credits!");

        return false;
    }
}
