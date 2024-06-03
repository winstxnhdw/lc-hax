#pragma warning disable IDE1006

#region

using HarmonyLib;
using Hax;
using Unity.Netcode;

#endregion

[HarmonyPatch]
class GrabbableDependencyPatch {
    [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.Update))]
    static void Postfix(GrabbableObject __instance) => Helper.Grabbables.Add(__instance);

    [HarmonyPatch(typeof(NetworkBehaviour), nameof(NetworkBehaviour.OnDestroy))]
    static void Prefix(NetworkBehaviour __instance) {
        if (__instance is GrabbableObject grabbableObject) _ = Helper.Grabbables.Remove(grabbableObject);
    }
}
