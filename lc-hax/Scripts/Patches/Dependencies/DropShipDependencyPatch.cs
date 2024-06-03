#region

using System;
using HarmonyLib;

#endregion

[HarmonyPatch(typeof(ItemDropship), "OpenShipClientRpc")]
class DropShipDependencyPatch {
    internal static event Action? OnDropShipDoorOpen;

    static void Postfix() => OnDropShipDoorOpen?.Invoke();
}
