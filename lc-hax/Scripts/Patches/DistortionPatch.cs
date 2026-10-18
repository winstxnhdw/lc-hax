using HarmonyLib;
using UnityEngine.Rendering.HighDefinition;

[HarmonyPatch(typeof(FrameSettings), "Sanitize")]
sealed class DistortionPatch {
    static void Prefix(ref FrameSettings sanitizedFrameSettings) =>
        sanitizedFrameSettings.SetEnabled(FrameSettingsField.Distortion, false);
}
