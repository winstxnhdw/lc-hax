using System;
using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

sealed class ClearVisionMod : MonoBehaviour {
    float LightIntensity { get; set; } = 2.0f;

    void OnEnable() {
        InputListener.OnF4Press += this.DecreaseLightIntensity;
        InputListener.OnF5Press += this.IncreaseLightIntensity;
    }

    void OnDisable() {
        InputListener.OnF4Press -= this.DecreaseLightIntensity;
        InputListener.OnF5Press -= this.IncreaseLightIntensity;
    }

    void IncreaseLightIntensity() => this.LightIntensity = Math.Clamp(this.LightIntensity + 1.0f, 0.0f, 10.0f);

    void DecreaseLightIntensity() => this.LightIntensity = Math.Clamp(this.LightIntensity - 1.0f, 0.0f, 10.0f);

    void Update() {
        if (Helper.LocalPlayer is not { } localPlayer) return;

        PlayerControllerB viewedPlayer = localPlayer.isPlayerDead
            ? localPlayer.spectatedPlayerScript ?? localPlayer
            : localPlayer;

        viewedPlayer.nightVision.intensity = this.LightIntensity;
    }

    void LateUpdate() {
        if (Helper.StartOfRound is not { } startOfRound) return;
        if (Helper.CurrentCamera is not { } camera) return;
        if (Helper.TimeOfDay is not { } timeOfDay) return;
        if (timeOfDay.sunAnimator.Unfake() is not { } sunAnimator) return;
        if (timeOfDay.sunIndirect.Unfake() is not { } sunIndirect) return;
        if (timeOfDay.sunDirect.Unfake() is not { } sunDirect) return;
        if (!sunIndirect.TryGetComponent(out HDAdditionalLightData lightData)) return;
        if (startOfRound.blackSkyVolume is Volume blackSkyVolume) {
            Destroy(blackSkyVolume);
        }

        sunAnimator.enabled = false;
        sunIndirect.transform.eulerAngles = new Vector3(90, 0, 0);
        sunIndirect.transform.position = camera.transform.position;
        sunIndirect.color = Color.white;
        sunIndirect.intensity = this.LightIntensity;
        sunIndirect.enabled = true;
        sunDirect.transform.eulerAngles = new Vector3(90, 0, 0);
        sunDirect.enabled = true;
        lightData.lightDimmer = float.MaxValue;
        lightData.distance = float.MaxValue;
        timeOfDay.insideLighting = false;
    }
}
