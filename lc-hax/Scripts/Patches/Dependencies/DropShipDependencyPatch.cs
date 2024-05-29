using System;
using HarmonyLib;

[HarmonyPatch(typeof(ItemDropship), "OpenShipClientRpc")]
internal class DropShipDependencyPatch
{
    internal static event Action? OnDropShipDoorOpen;

    private static void Postfix()
    {
        OnDropShipDoorOpen?.Invoke();
    }
}