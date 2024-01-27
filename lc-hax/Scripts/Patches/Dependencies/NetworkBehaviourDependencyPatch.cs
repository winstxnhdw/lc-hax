#pragma warning disable IDE1006

using HarmonyLib;
using Hax;
using Unity.Netcode;

[HarmonyPatch(typeof(NetworkBehaviour))]
class NetworkBehaviourDependencyPatch {

    [HarmonyPatch(nameof(NetworkBehaviour.OnDestroy))]
    static void Prefix(NetworkBehaviour __instance) {
        if (__instance is GrabbableObject grabbableObject)
            _ = Helper.Grabbables.Remove(grabbableObject);
    }
}
