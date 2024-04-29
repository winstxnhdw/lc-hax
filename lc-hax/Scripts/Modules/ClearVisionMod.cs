using System.Collections;
using Hax;
using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using GameNetcodeStuff;
using UnityEngine.UI;

sealed class ClearVisionMod : MonoBehaviour {

    internal static ClearVisionMod? Instance { get; private set; }
    bool IsInsideFactory => Helper.LocalPlayer is PlayerControllerB player && player.isInsideFactory;

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


    private void UpdateNewSun()
    {
        if (SunObject == null) return;
        if (SunLight == null) return;
        if (Helper.StartOfRound.inShipPhase)
        {
            SunLight.enabled = false;
            return;
        }
        if (Helper.TimeOfDay != null && Helper.TimeOfDay.sunIndirect != null)
        {
            SunLight.cullingMask = Helper.TimeOfDay.sunIndirect.cullingMask;
        }
        SunLight.enabled = enabled;
        SunLight.intensity = LightIntensity;

    }
    private void RemoveBlackSkybox()
    {
        if (Helper.StartOfRound.blackSkyVolume != null)
        {
            Helper.StartOfRound.blackSkyVolume.weight = 0f;
            UnityEngine.Object.Destroy(Helper.StartOfRound.blackSkyVolume);
        }
    }
    void ToggleFog(bool active)
    {
        HaxObjects.Instance?.LocalVolumetricFogs?.ForEach(localVolumetricFog => localVolumetricFog?.gameObject.SetActive(active));
    }

    void RemoveVisor()
    {
        if (Helper.LocalPlayer?.localVisor is not null && Helper.LocalPlayer.localVisor.gameObject.activeSelf)
        {
            Helper.LocalPlayer.localVisor.gameObject.SetActive(false);
        }
    }


    private void Update()
    {
        UpdateNewSun();
        RemoveBlackSkybox();
        RemoveVisor();
        ToggleFog(false);
    }


    void IncreaseLightIntensity() {
        this.LightIntensity = Math.Clamp(this.LightIntensity + 1.0f, LightIntensity_Min, LightIntensity_Max);
        Console.WriteLine($"LightIntensity: {this.LightIntensity}");
    }

    void DecreaseLightIntensity() {
        this.LightIntensity = Math.Clamp(this.LightIntensity - 1.0f, LightIntensity_Min, LightIntensity_Max);
        Console.WriteLine($"LightIntensity: {this.LightIntensity}");
    }



    void DisableMod()
    {
        if(SunLight is not null)
        {
            SunLight.enabled = false;
        }
        ToggleFog(true);
    }

    void OnEnable()
    {
        InputListener.OnF4Press += this.DecreaseLightIntensity;
        InputListener.OnF5Press += this.IncreaseLightIntensity;
    }

    void OnDisable()
    {
        InputListener.OnF4Press -= this.DecreaseLightIntensity;
        InputListener.OnF5Press -= this.IncreaseLightIntensity;
        DisableMod();
    }

}
