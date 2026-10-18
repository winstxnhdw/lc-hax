using HarmonyLib;
using UnityEngine.Rendering.HighDefinition;

[HarmonyPatch(typeof(FrameSettings), "Sanitize")]
sealed class CustomPassPatch {
    static void Prefix(ref FrameSettings sanitizedFrameSettings) =>
        sanitizedFrameSettings.SetEnabled(FrameSettingsField.CustomPass, false);
}
