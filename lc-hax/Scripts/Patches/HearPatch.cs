#pragma warning disable IDE1006

using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.UpdatePlayerVoiceEffects))]
sealed class HearPatch {
    static void Postfix(StartOfRound __instance) {
        if (!Setting.EnableEavesdrop) return;
        if (Helper.StartOfRound is { shipIsLeaving: true }) return;
        if (Helper.SoundManager is not SoundManager soundManager) return;

        __instance.allPlayerScripts.ForEach(player => {
            if (player == null || player.IsSelf()) return;
            if (!player.isPlayerControlled && !player.isPlayerDead) return;
            if (player.currentVoiceChatAudioSource.Unfake() is not AudioSource currentVoiceChatAudioSource) return;
            if (player.currentVoiceChatIngameSettings.Unfake() is not PlayerVoiceIngameSettings voiceSettings) return;
            if (player.voicePlayerState is not { } voiceState) return;

            currentVoiceChatAudioSource.panStereo = 0.0f;
            soundManager.playerVoicePitchTargets[player.playerClientId] = 1.0f;
            soundManager.SetPlayerPitch(1.0f, unchecked((int)player.playerClientId));

            currentVoiceChatAudioSource.spatialBlend = 0.0f;
            voiceSettings.set2D = true;
            voiceState.Volume = 1.0f;

            if (currentVoiceChatAudioSource.TryGetComponent(out AudioLowPassFilter audioLowPassFilter)) {
                audioLowPassFilter.enabled = false;
            }

            if (currentVoiceChatAudioSource.TryGetComponent(out AudioHighPassFilter audioHighPassFilter)) {
                audioHighPassFilter.enabled = false;
            }
        });
    }
}
