using HarmonyLib;
using UnityEngine.Rendering.HighDefinition;

[HarmonyPatch(typeof(FrameSettings), "Sanitize")]
sealed class FogPatch {
    static void Prefix(ref FrameSettings sanitizedFrameSettings) {
        sanitizedFrameSettings.SetEnabled(FrameSettingsField.AtmosphericScattering, false);
        sanitizedFrameSettings.SetEnabled(FrameSettingsField.Volumetrics, false);
        sanitizedFrameSettings.SetEnabled(FrameSettingsField.ReprojectionForVolumetrics, false);
    }
}
