using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("/respawn")]
public class RespawnCommand : ICommand {
    void RespawnLocalPlayer(PlayerControllerB localPlayer, StartOfRound startOfRound) {
        if (Helper.HUDManager is not HUDManager hudManager) return;
        if (Helper.SoundManager is not SoundManager soundManager) return;

        startOfRound.allPlayersDead = false;
        startOfRound.SetPlayerObjectExtrapolate(false);
        localPlayer.ResetPlayerBloodObjects();
        localPlayer.ResetZAndXRotation();
        localPlayer.isClimbingLadder = false;
        localPlayer.thisController.enabled = true;
        localPlayer.health = 100;
        localPlayer.disableLookInput = false;
        localPlayer.isPlayerDead = false;
        localPlayer.isPlayerControlled = true;
        localPlayer.isInElevator = true;
        localPlayer.isInHangarShipRoom = true;
        localPlayer.isInsideFactory = false;
        localPlayer.wasInElevatorLastFrame = false;
        localPlayer.setPositionOfDeadPlayer = false;
        localPlayer.TeleportPlayer(startOfRound.playerSpawnPositions[0].position, false, 0.0f, false, true);
        localPlayer.DisablePlayerModel(startOfRound.allPlayerObjects[localPlayer.playerClientId], true, true);

        if (localPlayer.TryGetComponent(out Light helmetLight)) {
            helmetLight.enabled = false;
        }

        localPlayer.Crouch(false);
        localPlayer.playerBodyAnimator?.SetBool("Limp", false);
        localPlayer.criticallyInjured = false;
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
        localPlayer.sinkingValue = 0.0f;

        if (localPlayer.TryGetComponent(out AudioSource statusEffectAudio)) {
            statusEffectAudio.Stop();
        }

        localPlayer.DisableJetpackControlsLocally();
        localPlayer.health = 100;
        localPlayer.mapRadarDotAnimator?.SetBool("dead", false);

        if (localPlayer.IsOwner) {
            if (hudManager.TryGetComponent(out Animator gasHelmetAnimator)) {
                gasHelmetAnimator.SetBool("gasEmitting", false);
            }

            localPlayer.hasBegunSpectating = false;
            hudManager.RemoveSpectateUI();

            if (hudManager.TryGetComponent(out Animator gameOverAnimator)) {
                gameOverAnimator.SetTrigger("revive");
            }

            localPlayer.hinderedMultiplier = 1.0f;
            localPlayer.isMovementHindered = 0;
            localPlayer.sourcesCausingSinking = 0;
            localPlayer.reverbPreset = startOfRound.shipReverb;
        }

        soundManager.earsRingingTimer = 0.0f;
        localPlayer.voiceMuffledByEnemy = false;
        soundManager.playerVoicePitchTargets[localPlayer.playerClientId] = 1.0f;
        soundManager.SetPlayerPitch(1.0f, localPlayer.PlayerIndex());

        if (localPlayer.currentVoiceChatIngameSettings == null) {
            startOfRound.RefreshPlayerVoicePlaybackObjects();
        }

        if (localPlayer.currentVoiceChatIngameSettings != null) {
            if (localPlayer.currentVoiceChatIngameSettings.voiceAudio == null) {
                localPlayer.currentVoiceChatIngameSettings.InitializeComponents();
            }

            if (localPlayer.currentVoiceChatIngameSettings.voiceAudio is AudioSource voiceAudio) {
                if (voiceAudio.TryGetComponent(out OccludeAudio occludeAudio)) {
                    occludeAudio.overridingLowPass = false;
                }
            }
        }
    }

    public void Execute(StringArray _) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (!localPlayer.isPlayerDead) {
            Chat.Print("You are not yet dead!");
            return;
        }

        this.RespawnLocalPlayer(localPlayer, startOfRound);

        Helper.CreateComponent<WaitForBehaviour>("Respawn")
              .SetPredicate(() => startOfRound.shipIsLeaving)
              .Init(localPlayer.KillPlayer);

        Chat.Print("No one can see you in this state!");
    }
}
