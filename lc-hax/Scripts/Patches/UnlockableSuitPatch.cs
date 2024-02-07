using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(UnlockableSuit))]
internal static class UnlockableSuitPatch {
    private static bool overrideSuit = false;
    private static int customSuitID = -1;

    public static void SetPlayerSuit(Unlockable suitID) {
        if (Helper.LocalPlayer is null) return;
        customSuitID = (int)suitID;
        overrideSuit = true;
        UnlockableSuit[] unlockableSuits = UnityEngine.Object.FindObjectsOfType<UnlockableSuit>();
        foreach (UnlockableSuit suit in unlockableSuits) {
            if (suit.NetworkManager.IsListening) {
                suit.SwitchSuitToThis(Helper.LocalPlayer);
                break;
            }
        }
    }

    [HarmonyPatch("SwitchSuitToThis")]
    static void Prefix(UnlockableSuit __instance, ref int ___suitID, out int __state) {
        __state = ___suitID;
        if (overrideSuit) {
            ___suitID = customSuitID;
        }
    }

    [HarmonyPatch("SwitchSuitToThis")]
    static void Postfix(UnlockableSuit __instance, ref int ___suitID, int __state) {
        ___suitID = __state;
        overrideSuit = false;
    }
}
