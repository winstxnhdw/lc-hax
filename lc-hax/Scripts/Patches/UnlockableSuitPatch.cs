using HarmonyLib;
using Hax;
using System;
using UnityEngine;

[HarmonyPatch(typeof(UnlockableSuit))]
public static class UnlockableSuitPatch {

    private static bool overrideSuit = false;
    private static int customSuitID = -1;
    private static int originalSuitID = -1;

    public static void SetPlayerSuit(Unlockable suitID) {
        if (Helper.LocalPlayer is null) return;
        customSuitID = (int)suitID;
        originalSuitID = -1;
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
    static bool Prefix(UnlockableSuit __instance, ref int ___suitID) {
        if (overrideSuit) {
            originalSuitID = __instance.suitID;
            ___suitID = customSuitID;
        }
        return true;
    }

    [HarmonyPatch("SwitchSuitToThis")]
    static void Postfix(UnlockableSuit __instance, ref int ___suitID) {
        if (overrideSuit) {
            overrideSuit = false;
            ___suitID = originalSuitID;
        }
    }
}
