using System;
using HarmonyLib;

[HarmonyPatch(typeof(ItemDropship), "OpenShipClientRpc")]
class DropShipDependencyPatch
{
    internal static event Action? OnDropShipDoorOpen;

    static void Postfix() => DropShipDependencyPatch.OnDropShipDoorOpen?.Invoke();
}
