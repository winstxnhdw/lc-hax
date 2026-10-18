using HarmonyLib;
using UnityEngine.Rendering.HighDefinition;

[HarmonyPatch(typeof(FrameSettings), "Sanitize")]
sealed class PostProcessingPatch {
    static void Prefix(ref FrameSettings sanitizedFrameSettings) {
        sanitizedFrameSettings.SetEnabled(FrameSettingsField.Postprocess, false);
        sanitizedFrameSettings.SetEnabled(FrameSettingsField.CustomPass, false);
        sanitizedFrameSettings.SetEnabled(FrameSettingsField.Distortion, false);
    }
}
