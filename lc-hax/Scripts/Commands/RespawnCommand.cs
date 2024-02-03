using Hax;
using GameNetcodeStuff;

[Command("/respawn")]
public class RespawnCommand : ICommand {

    public void RespawnLocalPlayer() // This is a modified version of StartOfRound.ReviveDeadPlayers
       {
        PlayerControllerB? localPlayer = Helper.LocalPlayer;
        if (localPlayer == null || Helper.StartOfRound == null || Helper.HUDManager == null || Helper.SoundManager == null) return;

        Helper.StartOfRound.allPlayersDead = false;
        localPlayer.ResetPlayerBloodObjects(localPlayer.isPlayerDead);
        if (localPlayer.isPlayerDead || localPlayer.isPlayerControlled) {
            localPlayer.isClimbingLadder = false;
            localPlayer.ResetZAndXRotation();
            localPlayer.thisController.enabled = true;
            localPlayer.health = 100;
            localPlayer.disableLookInput = false;
            if (localPlayer.isPlayerDead) {
                localPlayer.isPlayerDead = false;
                localPlayer.isPlayerControlled = true;
                localPlayer.isInElevator = true;
                localPlayer.isInHangarShipRoom = true;
                localPlayer.isInsideFactory = false;
                localPlayer.wasInElevatorLastFrame = false;
                Helper.StartOfRound.SetPlayerObjectExtrapolate(false);
                localPlayer.TeleportPlayer(Helper.StartOfRound.playerSpawnPositions[0].position, false, 0f, false, true);
                localPlayer.setPositionOfDeadPlayer = false;
                localPlayer.DisablePlayerModel(Helper.StartOfRound.allPlayerObjects[localPlayer.playerClientId], true, true);
                localPlayer.helmetLight.enabled = false;
                localPlayer.Crouch(false);
                localPlayer.criticallyInjured = false;
                localPlayer.playerBodyAnimator?.SetBool("Limp", false);
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
                localPlayer.statusEffectAudio.Stop();
                localPlayer.DisableJetpackControlsLocally();
                localPlayer.health = 100;
                localPlayer.mapRadarDotAnimator.SetBool("dead", false);
                if (localPlayer.IsOwner) {
                    Helper.HUDManager.gasHelmetAnimator.SetBool("gasEmitting", false);
                    localPlayer.hasBegunSpectating = false;
                    Helper.HUDManager.RemoveSpectateUI();
                    Helper.HUDManager.gameOverAnimator.SetTrigger("revive");
                    localPlayer.hinderedMultiplier = 1f;
                    localPlayer.isMovementHindered = 0;
                    localPlayer.sourcesCausingSinking = 0;
                    localPlayer.reverbPreset = Helper.StartOfRound.shipReverb;
                }
            }
            Helper.SoundManager.earsRingingTimer = 0f;
            localPlayer.voiceMuffledByEnemy = false;
            Helper.SoundManager.playerVoicePitchTargets[localPlayer.playerClientId] = 1f;
            Helper.SoundManager.SetPlayerPitch(1f, (int)localPlayer.playerClientId);
            if (localPlayer.currentVoiceChatIngameSettings == null) {
                Helper.StartOfRound.RefreshPlayerVoicePlaybackObjects();
            }
            if (localPlayer.currentVoiceChatIngameSettings != null) {
                if (localPlayer.currentVoiceChatIngameSettings.voiceAudio == null)
                    localPlayer.currentVoiceChatIngameSettings.InitializeComponents();

                if (localPlayer.currentVoiceChatIngameSettings.voiceAudio == null)
                    return;

                localPlayer.currentVoiceChatIngameSettings.voiceAudio.GetComponent<OccludeAudio>().overridingLowPass = false;
            }
        }
        PlayerControllerB playerControllerB = GameNetworkManager.Instance.localPlayerController;
