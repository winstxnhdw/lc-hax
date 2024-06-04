#region

using System;
using GameNetcodeStuff;
using Hax;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

#endregion

sealed class ClearVisionMod : MonoBehaviour {
    internal static ClearVisionMod? Instance { get; private set; }
    EntranceTeleport? MainEntrance => RoundManager.FindMainEntranceScript(false);
    internal bool IsInsideFactory { get;  set; }
    internal bool IsDead => Helper.LocalPlayer is PlayerControllerB player && player.IsDead();

    internal float LightIntensity_Min => 0f;
    internal float LightIntensity_Max => 100f;

    internal float InternalLight { get; set; } = 15.0f;
    internal float OutsideLight { get; set; } = 0.5f;

    internal float LightIntensity {
        get {
            float Light = this.IsInsideFactory ? this.InternalLight : this.OutsideLight;
            if (this.IsDead) Light *= 2f;
            return Light;
        }
        set {
            if (this.IsInsideFactory)
                this.InternalLight = value;
            else
                this.OutsideLight = value;
        }
    }





    internal void DetectPosition() {
        if (Helper.CurrentCamera is not Camera cam) return;
        if (this.MainEntrance is not EntranceTeleport entrance) return;
        this.IsInsideFactory = cam.transform.position.y < entrance.transform.position.y + Setting.IsInsideFactoryTreshold;
    }

    internal GameObject SunObject { get; set; }
    internal Light SunLight { get; set; }
    internal HDAdditionalLightData Data { get; set; }

    void Awake() {
        if (Instance != null) {
            Destroy(this);
            return;
        }

        Instance = this;
        this.SpawnSun();
    }

    internal void SpawnSun() {
        if (this.SunObject is null) this.SunObject = new GameObject("Lc-Hax Sun");
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

    void UpdateNewSun() {
        if (this.SunObject == null) return;
        if (this.SunLight == null) return;
        if (this.Data == null) return;
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (Helper.TimeOfDay is not TimeOfDay timeOfDay) return;
        if (Helper.CurrentCamera is not Camera camera) return;
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

    void RemoveBlackSkybox() {
        if (Helper.StartOfRound is not StartOfRound round) return;

        if (round.blackSkyVolume != null) {
            round.blackSkyVolume.weight = 0f;
            Destroy(round.blackSkyVolume);
        }
    }

    void ToggleFog(bool active) =>
        HaxObjects.Instance?.LocalVolumetricFogs?.ForEach(localVolumetricFog =>
            localVolumetricFog?.gameObject.SetActive(active));

    void ToggleVisor(bool active) {
        if (Helper.LocalPlayer?.localVisor.gameObject is not GameObject visor) return;
        if (visor.activeSelf != active) visor.SetActive(active);
    }

    void Update() {
        this.DetectPosition();
        this.UpdateNewSun();
        this.RemoveBlackSkybox();
        this.ToggleVisor(false);
        this.ToggleFog(false);
    }

    void IncreaseLightIntensity() {
        this.LightIntensity = Math.Clamp(this.LightIntensity + 1.0f, this.LightIntensity_Min, this.LightIntensity_Max);
        Console.WriteLine($"LightIntensity: {this.LightIntensity}");
    }

    void DecreaseLightIntensity() {
        this.LightIntensity = Math.Clamp(this.LightIntensity - 1.0f, this.LightIntensity_Min, this.LightIntensity_Max);
        Console.WriteLine($"LightIntensity: {this.LightIntensity}");
    }

    void DisableMod() {
        if (this.SunLight is not null) this.SunLight.enabled = false;
        this.ToggleFog(true);
        this.ToggleVisor(true);
    }

    void OnEnable() {
        InputListener.OnF4Press += this.DecreaseLightIntensity;
        InputListener.OnF5Press += this.IncreaseLightIntensity;
    }

    void OnDisable() {
        InputListener.OnF4Press -= this.DecreaseLightIntensity;
        InputListener.OnF5Press -= this.IncreaseLightIntensity;
        this.DisableMod();
    }
}
