using System;
using System.Collections;
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
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;

        PlayerControllerB viewedPlayer = localPlayer.isPlayerDead
            ? localPlayer.spectatedPlayerScript ?? localPlayer
            : localPlayer;

        viewedPlayer.nightVision.intensity = this.LightIntensity;
    }

    void LateUpdate() {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (Helper.CurrentCamera is not Camera camera) return;
        if (Helper.TimeOfDay is not TimeOfDay timeOfDay) return;
        if (timeOfDay.sunAnimator.Unfake() is not Animator sunAnimator) return;
        if (timeOfDay.sunIndirect.Unfake() is not Light sunIndirect) return;
        if (timeOfDay.sunDirect.Unfake() is not Light sunDirect) return;
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

    IEnumerator DisableVisor(object[] args) {
        WaitForSeconds waitForTenSeconds = new(10.0f);

        while (true) {
            Helper.LocalPlayer?.localVisor.gameObject.SetActive(false);
            yield return waitForTenSeconds;
        }
    }

    IEnumerator DisableFog(object[] args) {
        WaitForSeconds waitForFiveSeconds = new(5.0f);

        while (true) {
            HaxObjects
                .Instance?
                .LocalVolumetricFogs?
                .ForEach(localVolumetricFog => localVolumetricFog.Unfake()?.gameObject.SetActive(false));

            yield return waitForFiveSeconds;
        }
    }

    IEnumerator DisableSteamValves(object[] args) {
        WaitForSeconds waitForFiveSeconds = new(5.0f);

        while (true) {
            Helper
                .RoundManager?
                .steamValves?
                .ForEach(
                    valve => valve.Unfake()?.valveSteamParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear)
                );

            yield return waitForFiveSeconds;
        }
    }

    void Start() {
        _ = this.StartResilientCoroutine(this.DisableFog);
        _ = this.StartResilientCoroutine(this.DisableSteamValves);
        _ = this.StartResilientCoroutine(this.DisableVisor);
    }
}
