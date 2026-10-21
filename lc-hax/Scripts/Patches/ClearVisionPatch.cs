using System;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Object = UnityEngine.Object;

[HarmonyPatch]
sealed class ClearVisionPatch {
    static bool EventsSubscribed { get; set; }
    static float LightIntensity { get; set; } = Setting.DefaultLightIntensity;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(InputListener), "Update")]
    static void RegisterInputHandlers() {
        if (ClearVisionPatch.EventsSubscribed) return;

        InputListener.OnF4Press += ClearVisionPatch.DecreaseLightIntensity;
        InputListener.OnF5Press += ClearVisionPatch.IncreaseLightIntensity;
        ClearVisionPatch.EventsSubscribed = true;
    }

    static void IncreaseLightIntensity() =>
        ClearVisionPatch.LightIntensity = Math.Clamp(ClearVisionPatch.LightIntensity + 1.0f, 0.0f, 10.0f);

    static void DecreaseLightIntensity() =>
        ClearVisionPatch.LightIntensity = Math.Clamp(ClearVisionPatch.LightIntensity - 1.0f, 0.0f, 10.0f);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.Update))]
    static void UpdateNightVision(PlayerControllerB __instance) {
        if (!ReferenceEquals(__instance, Helper.LocalPlayer)) return;

        PlayerControllerB viewedPlayer = __instance.isPlayerDead
            ? __instance.spectatedPlayerScript ?? __instance
            : __instance;

        viewedPlayer.nightVision.intensity = ClearVisionPatch.LightIntensity;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.LateUpdate))]
    static void UpdateLighting(StartOfRound __instance) {
        if (Helper.CurrentCamera is not { } camera) return;
        if (Helper.TimeOfDay is not { } timeOfDay) return;
        if (timeOfDay.sunAnimator.Unfake() is not { } sunAnimator) return;
        if (timeOfDay.sunIndirect.Unfake() is not { } sunIndirect) return;
        if (timeOfDay.sunDirect.Unfake() is not { } sunDirect) return;
        if (!sunIndirect.TryGetComponent(out HDAdditionalLightData lightData)) return;
        if (__instance.blackSkyVolume is Volume blackSkyVolume) {
            Object.Destroy(blackSkyVolume);
        }

        sunAnimator.enabled = false;
        sunIndirect.transform.eulerAngles = new Vector3(90, 0, 0);
        sunIndirect.transform.position = camera.transform.position;
        sunIndirect.color = Color.white;
        sunIndirect.intensity = ClearVisionPatch.LightIntensity;
        sunIndirect.enabled = true;
        sunDirect.transform.eulerAngles = new Vector3(90, 0, 0);
        sunDirect.enabled = true;
        lightData.lightDimmer = float.MaxValue;
        lightData.distance = float.MaxValue;
        timeOfDay.insideLighting = false;
    }
}
