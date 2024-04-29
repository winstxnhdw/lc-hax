using System.Collections;
using Hax;
using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using GameNetcodeStuff;
using UnityEngine.UI;

sealed class ClearVisionMod : MonoBehaviour {

    internal static ClearVisionMod? Instance { get; private set; }
    float LightIntensity_Min => 0f;
    float LightIntensity_Max => 35f;

    float InternalLight { get; set; } = 25.0f;
    float OutsideLight { get; set; } = 0.5f;
    float LightIntensity
    {
        get => this.IsInsideFactory ? this.InternalLight : this.OutsideLight;
        set
        {
            if (this.IsInsideFactory)
            {
                this.InternalLight = value;
            }
            else
            {
                this.OutsideLight = value;
            }
        }
    }

    internal GameObject SunObject { get; set; }
    internal Light SunLight { get; set; }


    private void Awake()
    {
        if (ClearVisionMod.Instance != null)
        {
            Destroy(this);
            return;
        }
        ClearVisionMod.Instance = this;
        Initalize();
        SpawnSun();
    }

    internal void SpawnSun()
    {
        if (SunObject is null)
        {
            SunObject = new GameObject("Lc-Hax Sun");
        }
        SunObject.transform.parent = null;
        DontDestroyOnLoad(SunObject);
        SunLight = SunObject.GetOrAddComponent<Light>();
        SunLight.type = LightType.Directional;
        SunLight.shape = LightShape.Cone;
        SunLight.color = Color.white;
        SunLight.transform.position = new Vector3(0F, 1000F, 0F);
        SunLight.transform.rotation = Quaternion.Euler(90F, 0F, 0F);
        var Data = SunLight.GetOrAddComponent<HDAdditionalLightData>();
        if(Data != null)
        {
            Data.lightDimmer = 1;
            Data.volumetricDimmer = 0;
            Data.SetLightDimmer(1, 0);
            Data.EnableShadows(false);
            Data.distance = float.MaxValue;
        }
    }


    private void Update()
    {
        if(SunObject == null) return;
        if(SunLight == null) return;
        if(Helper.StartOfRound.inShipPhase)
        {
            SunLight.enabled = false;
            return;
        }
        if(Helper.TimeOfDay != null && Helper.TimeOfDay.sunIndirect != null)
        {
            SunLight.cullingMask = Helper.TimeOfDay.sunIndirect.cullingMask;
        }
        SunLight.enabled = enabled;
        SunLight.intensity = LightIntensity;
    }

    bool IsInsideFactory => Helper.LocalPlayer is PlayerControllerB player && player.isInsideFactory;

    void IncreaseLightIntensity() {
        this.LightIntensity = Math.Clamp(this.LightIntensity + 1.0f, LightIntensity_Min, LightIntensity_Max);
        Console.WriteLine($"LightIntensity: {this.LightIntensity}");
    }

    void DecreaseLightIntensity() {
        this.LightIntensity = Math.Clamp(this.LightIntensity - 1.0f, LightIntensity_Min, LightIntensity_Max);
        Console.WriteLine($"LightIntensity: {this.LightIntensity}");
    }

    IEnumerator RemoveBlackSkybox(object[] args) {
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

            if(startOfRound.blackSkyVolume is not null)
            {
                startOfRound.blackSkyVolume.weight = 0f;
                UnityEngine.Object.Destroy(startOfRound.blackSkyVolume);
                // end the routine if the black sky volume is destroyed
                yield break;
            }

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
        if(NoblackSlybox is null)
        {
            NoblackSlybox = this.StartResilientCoroutine(this.RemoveBlackSkybox);
        }
        if (SunLight is not null)
        {
            SunLight.enabled = true;
        }
    }
    void HaltRoutines()
    {
        if (Fog is Coroutine fog)
        {
            this.StopCoroutine(fog);
            Fog = null;
        }
        if (NoblackSlybox is Coroutine nightVision)
        {
            this.StopCoroutine(nightVision);
            NoblackSlybox = null;
        }
    }

    void RestoreVision()
    {
        //if(Helper.TimeOfDay is not TimeOfDay timeOfDay) return;
        //if(timeOfDay.sunAnimator is not Animator sunAnimator) return;
        //sunAnimator.enabled = true;
        //timeOfDay.insideLighting = IsInsideFactory;
        if(SunLight is not null)
        {
            SunLight.enabled = false;
        }
        HaxObjects
    .Instance?
    .LocalVolumetricFogs?
    .ForEach(localVolumetricFog =>
        localVolumetricFog?.gameObject.SetActive(true)
    );

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
    Coroutine NoblackSlybox { get; set; }

}
