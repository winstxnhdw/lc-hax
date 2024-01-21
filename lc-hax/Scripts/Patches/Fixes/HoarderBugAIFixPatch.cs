#pragma warning disable IDE1006

using HarmonyLib;
using System;
using System.Reflection;
using Unity.Netcode;

[HarmonyPatch(typeof(HoarderBugAI))]
internal class HoarderBugAIFixPatch {

    [HarmonyPatch(nameof(HoarderBugAI.HitEnemy))]
    private static void Prefix(ref HoarderBugAI __instance) {
        if (__instance is null) return;
        if (__instance.isEnemyDead || __instance.enemyHP <= 0) {
            if (__instance.heldItem != null) {
                InvokeDropItemAndCallDropRPC(__instance, __instance.heldItem.itemGrabbableObject.GetComponent<NetworkObject>(), false);
            }
        }
    }

    private static void InvokeDropItemAndCallDropRPC(HoarderBugAI bug, NetworkObject item, bool droppedInNest) {
        if (bug == null || item == null) {
            return;
        }
        if (DropItemAndCallDropRPCMethod != null) {
            object[] parameters = [item, droppedInNest];
            _ = DropItemAndCallDropRPCMethod.Invoke(bug, parameters);
        }
        else {
            Console.WriteLine("DropItemAndCallDropRPC Method not found.");
        }
    }

    private static MethodInfo? DropItemAndCallDropRPCMethod => typeof(HoarderBugAI).GetMethod("DropItemAndCallDropRPC", BindingFlags.NonPublic | BindingFlags.Instance);

}
