#pragma warning disable IDE1006

using HarmonyLib;
using Hax;
using Unity.Netcode;

[HarmonyPatch]
internal class GrabbableDependencyPatch
{
    [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.Update))]
    private static void Postfix(GrabbableObject __instance)
    {
        Helper.Grabbables.Add(__instance);
    }

    [HarmonyPatch(typeof(NetworkBehaviour), nameof(NetworkBehaviour.OnDestroy))]
    private static void Prefix(NetworkBehaviour __instance)
    {
        if (__instance is GrabbableObject grabbableObject) _ = Helper.Grabbables.Remove(grabbableObject);
    }
}