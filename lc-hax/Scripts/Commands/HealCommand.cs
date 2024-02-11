using Hax;

[Command("heal")]
internal class HealCommand : IStun, ICommand {
    void StunAtPlayerPosition(PlayerControllerB player) => this.Stun(player.transform.position, 5.0f, 1.0f);

    void RespawnLocalPlayer(PlayerControllerB localPlayer, StartOfRound startOfRound, HUDManager hudManager) {
        if (Helper.SoundManager is not SoundManager soundManager) return;

        startOfRound.allPlayersDead = false;
        startOfRound.SetPlayerObjectExtrapolate(false);
        localPlayer.ResetPlayerBloodObjects();
        localPlayer.ResetZAndXRotation();
        localPlayer.isClimbingLadder = false;
        localPlayer.thisController.enabled = true;
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

    Result HealLocalPlayer(HUDManager hudManager, StartOfRound startOfRound) {
        if (hudManager.localPlayer.health <= 0) {
            this.RespawnLocalPlayer(hudManager.localPlayer, startOfRound, hudManager);

            Helper.CreateComponent<WaitForBehaviour>("Respawn")
                  .SetPredicate(() => startOfRound.shipIsLeaving)
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

        this.StunAtPlayerPosition(hudManager.localPlayer);
        return new Result(true);
    }

    Result HealPlayer(StringArray args) {
        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            return new Result(message: "Target player is not alive or found!");
        }

        targetPlayer.HealPlayer();
        this.StunAtPlayerPosition(targetPlayer);

        return new Result(true);
    }

    public void Execute(StringArray args) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (Helper.HUDManager is not HUDManager hudManager) return;

        Result result = args.Length switch {
            0 => this.HealLocalPlayer(hudManager, startOfRound),
            _ => this.HealPlayer(args)
        };

        if (!result.Success) {
            Chat.Print(result.Message);
        }
    }
}
