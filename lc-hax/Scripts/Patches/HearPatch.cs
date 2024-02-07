#pragma warning disable IDE1006

using HarmonyLib;
using UnityEngine;
using Hax;

[HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.UpdatePlayerVoiceEffects))]
class HearPatch {
    static void Postfix(StartOfRound __instance) {
        if (!Setting.EnableEavesdrop || Helper.StartOfRound?.shipIsLeaving is true) return;
        if (Helper.SoundManager is not SoundManager soundManager) return;

        __instance.allPlayerScripts.ForEach(player => {
            AudioSource currentVoiceChatAudioSource = player.currentVoiceChatAudioSource;

            if (!currentVoiceChatAudioSource.TryGetComponent(out AudioLowPassFilter audioLowPassFilter)) {
                return;
            }

            if (!currentVoiceChatAudioSource.TryGetComponent(out AudioHighPassFilter audioHighPassFilter)) {
                return;
            }

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
