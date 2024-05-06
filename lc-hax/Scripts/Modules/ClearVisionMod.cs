using GameNetcodeStuff;
using Hax;
using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

internal sealed class ClearVisionMod : MonoBehaviour
{
    internal static ClearVisionMod? Instance { get; private set; }
    private bool IsInsideFactory => Helper.LocalPlayer is PlayerControllerB player && player.isInsideFactory;

    private float LightIntensity_Min => 0f;
    private float LightIntensity_Max => 35f;

    private float InternalLight { get; set; } = 25.0f;
    private float OutsideLight { get; set; } = 0.5f;

    private float LightIntensity
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
    internal HDAdditionalLightData Data { get; set; }
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
        Data = SunLight.GetOrAddComponent<HDAdditionalLightData>();
        if (Data != null)
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
        if(Data == null) return;
        if(Helper.LocalPlayer is not PlayerControllerB player) return;
        if(Helper.TimeOfDay is not TimeOfDay timeOfDay) return;
        if(Helper.CurrentCamera is not Camera camera) return;
        if (Helper.StartOfRound is not StartOfRound round)
        {
            SunLight.enabled = false;
            return;
        }
        if (round.inShipPhase)
        {
            SunLight.enabled = false;
            return;
        }
        UpdateCullingMask(camera);
        SunLight.enabled = this.enabled;
        if(player.IsDead())
        {
            SunLight.intensity = LightIntensity * 0.8f;
        }
        else
        {
            SunLight.intensity = LightIntensity;

        }
    }

    // Update the Culling Mask of the Sun Light to match the current camera
    void UpdateCullingMask(Camera currentCamera)
    {
        if (SunLight == null || Data == null || currentCamera == null)
            return;

        if (SunLight.cullingMask != currentCamera.cullingMask)
        {
            SunLight.cullingMask = currentCamera.cullingMask;
            Data.SetCullingMask(currentCamera.cullingMask);
        }
    }
    private void RemoveBlackSkybox()
    {
        if (Helper.StartOfRound is not StartOfRound round) return;

        if (round.blackSkyVolume != null)
        {
            round.blackSkyVolume.weight = 0f;
            UnityEngine.Object.Destroy(round.blackSkyVolume);
        }
    }

    private void ToggleFog(bool active)
    {
        HaxObjects.Instance?.LocalVolumetricFogs?.ForEach(localVolumetricFog => localVolumetricFog?.gameObject.SetActive(active));
    }

    private void RemoveVisor()
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

    private void IncreaseLightIntensity()
    {
        this.LightIntensity = Math.Clamp(this.LightIntensity + 1.0f, LightIntensity_Min, LightIntensity_Max);
        Console.WriteLine($"LightIntensity: {this.LightIntensity}");
    }

    private void DecreaseLightIntensity()
    {
        this.LightIntensity = Math.Clamp(this.LightIntensity - 1.0f, LightIntensity_Min, LightIntensity_Max);
        Console.WriteLine($"LightIntensity: {this.LightIntensity}");
    }

    private void DisableMod()
    {
        if (SunLight is not null)
        {
            SunLight.enabled = false;
        }
        ToggleFog(true);
    }

    private void OnEnable()
    {
        InputListener.OnF4Press += this.DecreaseLightIntensity;
        InputListener.OnF5Press += this.IncreaseLightIntensity;
    }

    private void OnDisable()
    {
        InputListener.OnF4Press -= this.DecreaseLightIntensity;
        InputListener.OnF5Press -= this.IncreaseLightIntensity;
        DisableMod();
    }
}