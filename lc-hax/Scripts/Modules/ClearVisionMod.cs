using System.Collections;
using Hax;
using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using GameNetcodeStuff;

sealed class ClearVisionMod : MonoBehaviour {

    internal static ClearVisionMod? Instance { get; private set; }
    float InternalLight { get; set; } = 2.0f;
    float OutsideLight { get; set; } = 0.5f;
    float LightIntensity {
        get => this.IsInsideFactory ? this.InternalLight : this.OutsideLight;
        set {
            if (this.IsInsideFactory) {
                this.InternalLight = value;
            }
            else {
                this.OutsideLight = value;
            }
        }
    }
    bool IsInsideFactory => Helper.LocalPlayer is PlayerControllerB player && player.isInsideFactory;

    void IncreaseLightIntensity() {
        this.LightIntensity = Math.Clamp(this.LightIntensity + 1.0f, 0.0f, 10.0f);
        Console.WriteLine($"LightIntensity: {this.LightIntensity}");
    }

    void DecreaseLightIntensity() {
        this.LightIntensity = Math.Clamp(this.LightIntensity - 1.0f, 0.0f, 10.0f);
        Console.WriteLine($"LightIntensity: {this.LightIntensity}");
    }

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
            startOfRound.blackSkyVolume.weight = 0;

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

    void Initalize()
    {
        if(Fog is null)
        {
            Fog = this.StartResilientCoroutine(this.DisableFog);
        }
        if(Visor is null)
        {
            Visor = this.StartResilientCoroutine(this.DisableVisor);
        }
        if(NightVision is null)
        {
            NightVision = this.StartResilientCoroutine(this.SetNightVision);
        }
    }
    void Start() {
        if (ClearVisionMod.Instance != null)
        {
            Destroy(this);
            return;
        }
        ClearVisionMod.Instance = this;
        this.Initalize();
    }

    void HaltRoutines()
    {
        if (Fog is Coroutine fog)
        {
            this.StopCoroutine(fog);
            Fog = null;
        }
        if (NightVision is Coroutine nightVision)
        {
            this.StopCoroutine(nightVision);
            NightVision = null;
        }
    }

    void RestoreVision()
    {
        if(Helper.TimeOfDay is not TimeOfDay timeOfDay) return;
        if(timeOfDay.sunAnimator is not Animator sunAnimator) return;
        sunAnimator.enabled = true;
        timeOfDay.insideLighting = IsInsideFactory;
    }

    void OnEnable()
    {
        InputListener.OnF4Press += this.DecreaseLightIntensity;
        InputListener.OnF5Press += this.IncreaseLightIntensity;
        this.Initalize();
    }

    void OnDisable()
    {
        InputListener.OnF4Press -= this.DecreaseLightIntensity;
        InputListener.OnF5Press -= this.IncreaseLightIntensity;
        HaltRoutines();
        RestoreVision();
    }

    Coroutine Fog { get; set; }
    Coroutine Visor { get; set; }
    Coroutine NightVision { get; set; }

}
