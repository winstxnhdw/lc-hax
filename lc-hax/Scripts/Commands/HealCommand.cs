using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("heal")]
class HealCommand : ICommand, IStun {
    void RespawnLocalPlayer(PlayerControllerB localPlayer, StartOfRound startOfRound, HUDManager hudManager) {
        if (Helper.SoundManager is not SoundManager soundManager) return;
        startOfRound.allPlayersDead = false;
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
                startOfRound.SetPlayerObjectExtrapolate(false);
                localPlayer.TeleportPlayer(startOfRound.playerSpawnPositions[0].position, false, 0f, false, true);
                localPlayer.setPositionOfDeadPlayer = false;
                localPlayer.DisablePlayerModel(startOfRound.allPlayerObjects[localPlayer.playerClientId], true, true);
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
                    hudManager.gasHelmetAnimator.SetBool("gasEmitting", false);
                    localPlayer.hasBegunSpectating = false;
                    hudManager.RemoveSpectateUI();
                    hudManager.gameOverAnimator.SetTrigger("revive");
                    localPlayer.hinderedMultiplier = 1f;
                    localPlayer.isMovementHindered = 0;
                    localPlayer.sourcesCausingSinking = 0;
                    localPlayer.reverbPreset = startOfRound.shipReverb;
                }
            }
            soundManager.earsRingingTimer = 0f;
            localPlayer.voiceMuffledByEnemy = false;
            soundManager.playerVoicePitchTargets[localPlayer.playerClientId] = 1f;
            soundManager.SetPlayerPitch(1f, (int)localPlayer.playerClientId);
            if (localPlayer.currentVoiceChatIngameSettings == null) {
                startOfRound.RefreshPlayerVoicePlaybackObjects();
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
        playerControllerB.bleedingHeavily = false;
        playerControllerB.criticallyInjured = false;
        playerControllerB.playerBodyAnimator.SetBool("Limp", false);
        playerControllerB.health = 100;
        hudManager.UpdateHealthUI(100, false);
        playerControllerB.spectatedPlayerScript = null;
        hudManager.audioListenerLowPass.enabled = false;
        startOfRound.SetSpectateCameraToGameOverMode(false, playerControllerB);
        RagdollGrabbableObject[] array = UnityEngine.Object.FindObjectsOfType<RagdollGrabbableObject>();
        for (int j = 0; j < array.Length; j++) {
            if (!array[j].isHeld) {
                if (startOfRound.IsServer) {
                    if (array[j].NetworkObject.IsSpawned)
                        array[j].NetworkObject.Despawn(true);
                    else
                        UnityEngine.Object.Destroy(array[j].gameObject);
                }
            }
            else if (array[j].isHeld && array[j].playerHeldBy != null) {
                array[j].playerHeldBy.DropAllHeldItems(true, false);
            }
        }
        DeadBodyInfo[] array2 = UnityEngine.Object.FindObjectsOfType<DeadBodyInfo>();
        for (int k = 0; k < array2.Length; k++) {
            UnityEngine.Object.Destroy(array2[k].gameObject);
        }
        startOfRound.livingPlayers = startOfRound.connectedPlayersAmount + 1;
        startOfRound.allPlayersDead = false;
        startOfRound.UpdatePlayerVoiceEffects();
        startOfRound.shipAnimator.ResetTrigger("ShipLeave");
    }



    PlayerControllerB HealLocalPlayer(HUDManager hudManager) {
        if (!hudManager.localPlayer.isPlayerControlled) {
            this.RespawnLocalPlayer(hudManager.localPlayer, hudManager.localPlayer.playersManager, hudManager);

            Helper.CreateComponent<WaitForBehaviour>("Respawn")
                  .SetPredicate(() => hudManager.localPlayer.playersManager.shipIsLeaving)
                  .Init(hudManager.localPlayer.KillPlayer);
        }

        hudManager.localPlayer.health = 100;
        hudManager.localPlayer.bleedingHeavily = false;
        hudManager.localPlayer.criticallyInjured = false;
        hudManager.localPlayer.hasBeenCriticallyInjured = false;
        hudManager.localPlayer.playerBodyAnimator.SetBool("Limp", false);
        hudManager.HUDAnimator.SetBool("biohazardDamage", false);
        hudManager.HUDAnimator.SetTrigger("HealFromCritical");
        hudManager.UpdateHealthUI(hudManager.localPlayer.health, false);

        return hudManager.localPlayer;
    }

    PlayerControllerB? HealPlayer(string? playerNameOrId) {
        PlayerControllerB? targetPlayer = Helper.GetActivePlayer(playerNameOrId);
        targetPlayer?.HealPlayer();

        return targetPlayer;
    }

    public void Execute(StringArray args) {
        if (Helper.HUDManager is not HUDManager hudManager) return;

        PlayerControllerB? healedPlayer = args.Length switch {
            0 => this.HealLocalPlayer(hudManager),
            _ => this.HealPlayer(args[0])
        };

        if (healedPlayer is null) {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        this.Stun(healedPlayer.transform.position, 5.0f, 1.0f);
    }
}
