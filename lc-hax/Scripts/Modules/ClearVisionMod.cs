using System;
using GameNetcodeStuff;
using Hax;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

internal sealed class ClearVisionMod : MonoBehaviour
{
    internal static ClearVisionMod? Instance { get; private set; }
    private bool IsInsideFactory => Helper.LocalPlayer is PlayerControllerB player && player.isInsideFactory;
    private bool IsDead => Helper.LocalPlayer is PlayerControllerB player && player.IsDead();

    private float LightIntensity_Min => 0f;
    private float LightIntensity_Max => 35f;

    private float InternalLight { get; set; } = 25.0f;
    private float OutsideLight { get; set; } = 0.5f;

    private float LightIntensity
    {
        get
        {
            var Light = IsInsideFactory ? InternalLight : OutsideLight;
            if (IsDead) Light *= 15f;
            return Light;
        }
        set
        {
            if (IsInsideFactory)
                InternalLight = value;
            else
                OutsideLight = value;
        }
    }

    internal GameObject SunObject { get; set; }
    internal Light SunLight { get; set; }
    internal HDAdditionalLightData Data { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        SpawnSun();
    }

    internal void SpawnSun()
    {
        if (SunObject is null) SunObject = new GameObject("Lc-Hax Sun");
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
        if (Data == null) return;
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (Helper.TimeOfDay is not TimeOfDay timeOfDay) return;
        if (Helper.CurrentCamera is not Camera camera) return;
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

        SunLight.enabled = enabled;
        SunLight.intensity = LightIntensity;
    }

    private void RemoveBlackSkybox()
    {
        if (Helper.StartOfRound is not StartOfRound round) return;

        if (round.blackSkyVolume != null)
        {
            round.blackSkyVolume.weight = 0f;
            Destroy(round.blackSkyVolume);
        }
    }

    private void ToggleFog(bool active)
    {
        HaxObjects.Instance?.LocalVolumetricFogs?.ForEach(localVolumetricFog =>
            localVolumetricFog?.gameObject.SetActive(active));
    }

    private void ToggleVisor(bool active)
    {
        if (Helper.LocalPlayer?.localVisor.gameObject is not GameObject visor) return;
        if (visor.activeSelf != active) visor.SetActive(active);
    }

    private void Update()
    {
        UpdateNewSun();
        RemoveBlackSkybox();
        ToggleVisor(false);
        ToggleFog(false);
    }

    private void IncreaseLightIntensity()
    {
        LightIntensity = Math.Clamp(LightIntensity + 1.0f, LightIntensity_Min, LightIntensity_Max);
        Console.WriteLine($"LightIntensity: {LightIntensity}");
    }

    private void DecreaseLightIntensity()
    {
        LightIntensity = Math.Clamp(LightIntensity - 1.0f, LightIntensity_Min, LightIntensity_Max);
        Console.WriteLine($"LightIntensity: {LightIntensity}");
    }

    private void DisableMod()
    {
        if (SunLight is not null) SunLight.enabled = false;
        ToggleFog(true);
        ToggleVisor(true);
    }

    private void OnEnable()
    {
        InputListener.OnF4Press += DecreaseLightIntensity;
        InputListener.OnF5Press += IncreaseLightIntensity;
    }

    private void OnDisable()
    {
        InputListener.OnF4Press -= DecreaseLightIntensity;
        InputListener.OnF5Press -= IncreaseLightIntensity;
        DisableMod();
    }
}