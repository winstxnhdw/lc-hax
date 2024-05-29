using GameNetcodeStuff;
using HarmonyLib;
using Hax;
using UnityEngine;

[HarmonyPatch(typeof(SandSpiderWebTrap))]
[HarmonyPatch("OnTriggerStay")]
internal class SandSpiderWebTrapPatch
{
    private static bool Prefix(SandSpiderWebTrap __instance, Collider other)
    {
        if (!Setting.DisableSpiderWebSlowness) return true;
        if (other.TryGetComponent<PlayerControllerB>(out var player))
            if (player != null)
                if (player.IsSelf())
                {
                    var reflect = __instance.Reflect();
                    _ = reflect.SetInternalField("hinderingLocalPlayer", true);
                    if (reflect.GetInternalField("currentTrappedPlayer") == null)
                        _ = reflect.SetInternalField("currentTrappedPlayer", player);
                    __instance.mainScript?.PlayerTripWebServerRpc(__instance.trapID, (int)player.actualClientId);
                    return false;
                }

        return true;
    }
}