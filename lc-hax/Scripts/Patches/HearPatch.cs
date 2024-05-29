#pragma warning disable IDE1006

using HarmonyLib;
using Hax;
using UnityEngine;

[HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.UpdatePlayerVoiceEffects))]
internal class HearPatch
{
    private static void Postfix(StartOfRound __instance)
    {
        if (!Setting.EnableEavesdrop) return;
        if (Helper.StartOfRound is { shipIsLeaving: true }) return;
        if (Helper.SoundManager is not SoundManager soundManager) return;

        __instance.allPlayerScripts.ForEach(player =>
        {
            var currentVoiceChatAudioSource = player.currentVoiceChatAudioSource;

            if (!currentVoiceChatAudioSource.TryGetComponent(out AudioLowPassFilter audioLowPassFilter)) return;

            if (!currentVoiceChatAudioSource.TryGetComponent(out AudioHighPassFilter audioHighPassFilter)) return;

            audioLowPassFilter.enabled = false;
            audioHighPassFilter.enabled = false;
            currentVoiceChatAudioSource.panStereo = 0.0f;
            soundManager.playerVoicePitchTargets[player.playerClientId] = 1.0f;
            soundManager.SetPlayerPitch(1.0f, unchecked((int)player.playerClientId));

            currentVoiceChatAudioSource.spatialBlend = 0.0f;
            player.currentVoiceChatIngameSettings.set2D = true;
            player.voicePlayerState.Volume = 1.0f;
        });
    }
}