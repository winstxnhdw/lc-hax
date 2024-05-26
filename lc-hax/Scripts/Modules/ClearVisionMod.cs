using GameNetcodeStuff;
using Hax;
using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

internal sealed class ClearVisionMod : MonoBehaviour {
    internal static ClearVisionMod? Instance { get; private set; }
    private bool IsInsideFactory => Helper.LocalPlayer is PlayerControllerB player && player.isInsideFactory;
    private bool IsDead => Helper.LocalPlayer is PlayerControllerB player && player.IsDead();

    private float LightIntensity_Min => 0f;
    private float LightIntensity_Max => 35f;

    private float InternalLight { get; set; } = 25.0f;
    private float OutsideLight { get; set; } = 0.5f;

    private float LightIntensity {
        get {
            float Light = this.IsInsideFactory ? this.InternalLight : this.OutsideLight;
            if (this.IsDead) {
                Light *= 15f;
            }
            return Light;
        }
        set {
            if (this.IsInsideFactory) {
                this.InternalLight = value;
            }
            else {
                this.OutsideLight = value;
            }
        }
    }

    internal GameObject SunObject { get; set; }
    internal Light SunLight { get; set; }
    internal HDAdditionalLightData Data { get; set; }
    private void Awake() {
        if (ClearVisionMod.Instance != null) {
            Destroy(this);
            return;
        }
        ClearVisionMod.Instance = this;
        this.SpawnSun();
    }

    internal void SpawnSun() {
        this.SunObject ??= new GameObject("Lc-Hax Sun");
        this.SunObject.transform.parent = null;
        DontDestroyOnLoad(this.SunObject);
        this.SunLight = this.SunObject.GetOrAddComponent<Light>();
        this.SunLight.type = LightType.Directional;
        this.SunLight.shape = LightShape.Cone;
        this.SunLight.color = Color.white;
        this.SunLight.transform.position = new Vector3(0F, 1000F, 0F);
        this.SunLight.transform.rotation = Quaternion.Euler(90F, 0F, 0F);
        this.Data = this.SunLight.GetOrAddComponent<HDAdditionalLightData>();
        if (this.Data != null) {
            this.Data.lightDimmer = 1;
            this.Data.volumetricDimmer = 0;
            this.Data.SetLightDimmer(1, 0);
            this.Data.EnableShadows(false);
            this.Data.distance = float.MaxValue;
        }
    }

    private void UpdateNewSun() {
        if (this.SunObject == null) return;
        if (this.SunLight == null) return;
        if (this.Data == null) return;
        if (Helper.LocalPlayer is not PlayerControllerB) return;
        if (Helper.TimeOfDay is not TimeOfDay) return;
        if (Helper.CurrentCamera is not Camera) return;
        if (Helper.StartOfRound is not StartOfRound round) {
            this.SunLight.enabled = false;
            return;
        }
        if (round.inShipPhase) {
            this.SunLight.enabled = false;
            return;
        }
        this.SunLight.enabled = this.enabled;
        this.SunLight.intensity = this.LightIntensity;
    }

    private void RemoveBlackSkybox() {
        if (Helper.StartOfRound is not StartOfRound round) return;

        if (round.blackSkyVolume != null) {
            round.blackSkyVolume.weight = 0f;
            UnityEngine.Object.Destroy(round.blackSkyVolume);
        }
    }

    private void ToggleFog(bool active) => HaxObjects.Instance?.LocalVolumetricFogs?.ForEach(localVolumetricFog => localVolumetricFog?.gameObject.SetActive(active));

    private void ToggleVisor(bool active) {
        if (Helper.LocalPlayer?.localVisor.gameObject is not GameObject visor) return;
        if (visor.activeSelf != active) {
            visor.SetActive(active);
        }
    }

    private void Update() {
        this.UpdateNewSun();
        this.RemoveBlackSkybox();
        this.ToggleVisor(false);
        this.ToggleFog(false);
    }

    private void IncreaseLightIntensity() {
        this.LightIntensity = Math.Clamp(this.LightIntensity + 1.0f, this.LightIntensity_Min, this.LightIntensity_Max);
        Console.WriteLine($"LightIntensity: {this.LightIntensity}");
    }

    private void DecreaseLightIntensity() {
        this.LightIntensity = Math.Clamp(this.LightIntensity - 1.0f, this.LightIntensity_Min, this.LightIntensity_Max);
        Console.WriteLine($"LightIntensity: {this.LightIntensity}");
    }

    private void DisableMod() {
        if (this.SunLight is not null) {
            this.SunLight.enabled = false;
        }
        this.ToggleFog(true);
        this.ToggleVisor(true);
    }

    private void OnEnable() {
        InputListener.OnF4Press += this.DecreaseLightIntensity;
        InputListener.OnF5Press += this.IncreaseLightIntensity;
    }

    private void OnDisable() {
        InputListener.OnF4Press -= this.DecreaseLightIntensity;
        InputListener.OnF5Press -= this.IncreaseLightIntensity;
        this.DisableMod();
    }
}
