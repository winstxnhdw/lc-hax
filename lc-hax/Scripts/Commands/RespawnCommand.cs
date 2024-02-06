using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("/respawn")]
public class RespawnCommand : ICommand {

    public void RespawnLocalPlayer() {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (Helper.HUDManager is not HUDManager hudManager) return;
        if (Helper.SoundManager is not SoundManager soundManager) return;

        startOfRound.allPlayersDead = false;
        localPlayer.ResetPlayerBloodObjects();
        localPlayer.isClimbingLadder = false;
        localPlayer.ResetZAndXRotation();
        localPlayer.thisController.enabled = true;
        localPlayer.health = 100;
        localPlayer.disableLookInput = false;
        localPlayer.isPlayerDead = false;
        localPlayer.isPlayerControlled = true;
        localPlayer.isInElevator = true;
        localPlayer.isInHangarShipRoom = true;
        localPlayer.isInsideFactory = false;
        localPlayer.wasInElevatorLastFrame = false;
        startOfRound.SetPlayerObjectExtrapolate(false);
        localPlayer.TeleportPlayer(startOfRound.playerSpawnPositions[0].position, false, 0f, false, true);
        localPlayer.setPositionOfDeadPlayer = false;
        localPlayer.DisablePlayerModel(startOfRound.allPlayerObjects[localPlayer.playerClientId], true, true);

        if (localPlayer.TryGetComponent(out Light helmetLight)) {
            helmetLight.enabled = false;
        }

        localPlayer.Crouch(false);
        localPlayer.criticallyInjured = false;

        if (localPlayer.playerBodyAnimator != null) {
            localPlayer.playerBodyAnimator.SetBool("Limp", false);
        }

        localPlayer.bleedingHeavily = false;
        localPlayer.activatingItem = false;
        localPlayer.twoHanded = false;
        localPlayer.inSpecialInteractAnimation = false;
        localPlayer.disableSyncInAnimation = false;
        localPlayer.inAnimationWithEnemy = null;
        localPlayer.holdingWalkieTalkie = false;
        localPlayer.speakingToWalkieTalkie = false;
        localPlayer.isSinking = false;
        localPlayer.isUnderwater = false;
        localPlayer.sinkingValue = 0f;

        if (localPlayer.TryGetComponent(out AudioSource statusEffectAudio)) {
            statusEffectAudio.Stop();
        }

        localPlayer.DisableJetpackControlsLocally();
        localPlayer.health = 100;

        if (localPlayer.mapRadarDotAnimator != null) {
            localPlayer.mapRadarDotAnimator.SetBool("dead", false);
        }

        if (localPlayer.IsOwner) {
            if (hudManager.TryGetComponent(out Animator gasHelmetAnimator)) {
                gasHelmetAnimator.SetBool("gasEmitting", false);
            }

            localPlayer.hasBegunSpectating = false;
            hudManager.RemoveSpectateUI();
            if (hudManager.TryGetComponent(out Animator gameOverAnimator)) {
                gameOverAnimator.SetTrigger("revive");
            }

            localPlayer.hinderedMultiplier = 1f;
            localPlayer.isMovementHindered = 0;
            localPlayer.sourcesCausingSinking = 0;
            localPlayer.reverbPreset = startOfRound.shipReverb;
        }

        soundManager.earsRingingTimer = 0f;
        localPlayer.voiceMuffledByEnemy = false;
        soundManager.playerVoicePitchTargets[localPlayer.playerClientId] = 1f;
        soundManager.SetPlayerPitch(1f, (int)localPlayer.playerClientId);

        if (localPlayer.currentVoiceChatIngameSettings == null) {
            startOfRound.RefreshPlayerVoicePlaybackObjects();
        }

        if (localPlayer.currentVoiceChatIngameSettings != null) {
            if (localPlayer.currentVoiceChatIngameSettings.voiceAudio == null) {
                localPlayer.currentVoiceChatIngameSettings.InitializeComponents();
            }

            if (localPlayer.currentVoiceChatIngameSettings.voiceAudio != null) {
                if (localPlayer.currentVoiceChatIngameSettings.voiceAudio
                    .TryGetComponent(out OccludeAudio occludeAudio)) {
                    occludeAudio.overridingLowPass = false;
                }
            }
        }
    }

    public void Execute(StringArray _) {
        this.RespawnLocalPlayer();
        Chat.Print("No one can see you in this state!");
    }
}
