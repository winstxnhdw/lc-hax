using GameNetcodeStuff;
using UnityEngine;
using Hax;

[Command("heal")]
internal class HealCommand : IStun, ICommand {
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
        localPlayer.voiceMuffledByEnemy = false;

        soundManager.earsRingingTimer = 0.0f;
        soundManager.playerVoicePitchTargets[localPlayer.playerClientId] = 1.0f;
        soundManager.SetPlayerPitch(1.0f, localPlayer.PlayerIndex());

        if (localPlayer.currentVoiceChatIngameSettings == null) {
            startOfRound.RefreshPlayerVoicePlaybackObjects();
        }

        localPlayer.currentVoiceChatIngameSettings?.InitializeComponents();

        if (localPlayer.currentVoiceChatIngameSettings?.voiceAudio?.TryGetComponent(out OccludeAudio occludeAudio) is not true) {
            return;
        }

        occludeAudio.overridingLowPass = false;
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
