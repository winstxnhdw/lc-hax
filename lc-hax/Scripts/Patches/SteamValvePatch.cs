using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(SteamValveHazard), nameof(SteamValveHazard.Start))]
sealed class SteamValvePatch {
    static void Postfix(SteamValveHazard __instance) => SteamValvePatch.Apply(__instance);

    internal static void Apply(SteamValveHazard? valve) {
        if (valve.Unfake()?.valveSteamParticle.Unfake() is not { } steam) return;

        foreach (ParticleSystemRenderer renderer in steam.GetComponentsInChildren<ParticleSystemRenderer>(includeInactive: true)) {
            renderer.enabled = false;
        }

        steam.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
