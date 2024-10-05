using System;
using System.Collections;
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

    IEnumerator SetNightVision(object[] args) {
        WaitForEndOfFrame waitForEndOfFrame = new();

        while (true) {
            if (Helper.StartOfRound is not StartOfRound startOfRound) {
                yield return waitForEndOfFrame;
                continue;
            }

            if (Helper.CurrentCamera is not Camera camera) {
                yield return waitForEndOfFrame;
                continue;
            }

            if (Helper.TimeOfDay is not TimeOfDay timeOfDay) {
                yield return waitForEndOfFrame;
                continue;
            }

            if (timeOfDay.sunAnimator is not Animator sunAnimator) {
                yield return waitForEndOfFrame;
                continue;
            }

            if (!timeOfDay.sunIndirect.TryGetComponent(out HDAdditionalLightData lightData)) {
                yield return waitForEndOfFrame;
                continue;
            }

            if (startOfRound.blackSkyVolume is Volume blackSkyVolume) {
                Destroy(blackSkyVolume);
            }

            sunAnimator.enabled = false;
            timeOfDay.sunIndirect.transform.eulerAngles = new Vector3(90, 0, 0);
            timeOfDay.sunIndirect.transform.position = camera.transform.position;
            timeOfDay.sunIndirect.color = Color.white;
            timeOfDay.sunIndirect.intensity = this.LightIntensity;
            timeOfDay.sunIndirect.enabled = true;
            timeOfDay.sunDirect.transform.eulerAngles = new Vector3(90, 0, 0);
            timeOfDay.sunDirect.enabled = true;
            lightData.lightDimmer = float.MaxValue;
            lightData.distance = float.MaxValue;
            timeOfDay.insideLighting = false;

            yield return waitForEndOfFrame;
        }
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
                .ForEach(localVolumetricFog =>
                    localVolumetricFog?.gameObject.SetActive(false)
                );

            yield return waitForFiveSeconds;
        }
    }

    IEnumerator DisableSteamValves(object[] args) {
        WaitForSeconds waitForFiveSeconds = new(5.0f);

        while (true) {
            HaxObjects
                .Instance?
                .SteamValves?
                .ForEach(valve =>
                    valve?.valveSteamParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear)
                );

            yield return waitForFiveSeconds;
        }
    }

    void Start() {
        _ = this.StartResilientCoroutine(this.DisableFog);
        _ = this.StartResilientCoroutine(this.DisableSteamValves);
        _ = this.StartResilientCoroutine(this.DisableVisor);
        _ = this.StartResilientCoroutine(this.SetNightVision);
    }
}
