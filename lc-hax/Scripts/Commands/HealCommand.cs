using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("heal")]
sealed class HealCommand : ICommand, IStun {
    void RespawnLocalPlayer(PlayerControllerB localPlayer, StartOfRound startOfRound, HUDManager hudManager) {
        if (Helper.SoundManager is not { } soundManager) return;

        soundManager.earsRingingTimer = 0.0f;
        soundManager.playerVoicePitchTargets[localPlayer.playerClientId] = 1.0f;
        soundManager.SetPlayerPitch(1.0f, localPlayer.PlayerIndex());

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
        localPlayer.parentedToElevatorLastFrame = false;
        localPlayer.setPositionOfDeadPlayer = false;
        localPlayer.TeleportPlayer(startOfRound.playerSpawnPositions[0].position);
        localPlayer.DisablePlayerModel(startOfRound.allPlayerObjects[localPlayer.playerClientId], true, true);
        localPlayer.helmetLight.enabled = false;
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
        localPlayer.statusEffectAudio.Stop();
        localPlayer.DisableJetpackControlsLocally();
        localPlayer.mapRadarDotAnimator?.SetBool("dead", false);
        localPlayer.hasBegunSpectating = false;
        localPlayer.hinderedMultiplier = 1.0f;
        localPlayer.isMovementHindered = 0;
        localPlayer.sourcesCausingSinking = 0;
        localPlayer.reverbPreset = startOfRound.shipReverb;
        localPlayer.voiceMuffledByEnemy = false;

        hudManager.RemoveSpectateUI();
        hudManager.gasHelmetAnimator.SetBool("gasEmitting", false);
        hudManager.gameOverAnimator.SetTrigger("revive");

        if (localPlayer.currentVoiceChatIngameSettings == null) {
            startOfRound.RefreshPlayerVoicePlaybackObjects();
        }

        PlayerVoiceIngameSettings? voiceSettings = localPlayer.currentVoiceChatIngameSettings.Unfake();
        voiceSettings?.InitializeComponents();
        _ = voiceSettings?.voiceAudio?.GetComponent<OccludeAudio>()?.overridingLowPass = false;
    }

    PlayerControllerB HealLocalPlayer(HUDManager hudManager) {
        if (!hudManager.localPlayer.isPlayerControlled) {
            this.RespawnLocalPlayer(hudManager.localPlayer, hudManager.localPlayer.playersManager, hudManager);

            Helper.CreateComponent<WaitForBehaviour>("Respawn")
                  .SetPredicate(() => hudManager.localPlayer.playersManager.shipIsLeaving)
                  .Init(hudManager.localPlayer.KillPlayer);
        }

        hudManager.localPlayer.health = 100;
        hudManager.localPlayer.MakeCriticallyInjured(false);
        hudManager.localPlayer.hasBeenCriticallyInjured = false;
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

    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.HUDManager is not { } hudManager) return Task.CompletedTask;

        PlayerControllerB? healedPlayer = args.Length switch {
            0 => this.HealLocalPlayer(hudManager),
            _ => this.HealPlayer(args[0])
        };

        if (healedPlayer is null) {
            Chat.Print("Target player is not alive or found!");
            return Task.CompletedTask;
        }

        this.Stun(healedPlayer.transform.position, 5.0f, 1.0f);
        return Task.CompletedTask;
    }
}
