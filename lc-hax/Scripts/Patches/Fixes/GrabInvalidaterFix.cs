#region

using System;
using GameNetcodeStuff;
using HarmonyLib;
using Hax;
using Unity.Netcode;

#endregion

// Items tend to bug out sometimes, this is a fix to ignore the grab validation RPC flag.
[HarmonyBefore]
[HarmonyPatch(typeof(PlayerControllerB), "GrabObjectClientRpc")]
class GrabInvalidaterFix {
    static void Prefix(ref PlayerControllerB __instance, ref bool grabValidated, NetworkObjectReference grabbedObject) {
        if (!__instance.IsSelf()) return;
        if (!grabValidated) {
            Console.WriteLine("Grab invalidated flag has been ignored.");
            grabValidated = true;
        }
    }
}
