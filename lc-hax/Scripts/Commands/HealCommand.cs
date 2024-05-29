using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("heal")]
internal class HealCommand : ICommand, IStun
{
    public void Execute(StringArray args)
    {
        if (Helper.HUDManager is not HUDManager hudManager) return;

        var healedPlayer = args.Length switch
        {
            0 => HealLocalPlayer(hudManager),
            _ => HealPlayer(args[0])
        };

        if (healedPlayer is null)
        {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        this.Stun(healedPlayer.transform.position, 5.0f, 1.0f);
    }

    private void RespawnLocalPlayer(PlayerControllerB localPlayer, StartOfRound startOfRound, HUDManager hudManager)
    {
        if (Helper.SoundManager is not SoundManager soundManager) return;
        if (localPlayer is not PlayerControllerB) return;
        if (startOfRound is not StartOfRound) return;
        if (hudManager is not HUDManager) return;

        startOfRound.allPlayersDead = false;
        localPlayer.ResetPlayerBloodObjects(localPlayer.isPlayerDead);
        if (localPlayer.isPlayerDead || localPlayer.isPlayerControlled)
        {
            localPlayer.isClimbingLadder = false;
            localPlayer.ResetZAndXRotation();
            localPlayer.thisController.enabled = true;
            localPlayer.health = 100;
            localPlayer.disableLookInput = false;
            if (localPlayer.isPlayerDead)
            {
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
                if (localPlayer.IsOwner)
                {
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
            if (localPlayer.currentVoiceChatIngameSettings == null) startOfRound.RefreshPlayerVoicePlaybackObjects();
            if (localPlayer.currentVoiceChatIngameSettings != null)
            {
                if (localPlayer.currentVoiceChatIngameSettings.voiceAudio == null)
                    localPlayer.currentVoiceChatIngameSettings.InitializeComponents();

                if (localPlayer.currentVoiceChatIngameSettings.voiceAudio == null)
                    return;

                localPlayer.currentVoiceChatIngameSettings.voiceAudio.GetComponent<OccludeAudio>().overridingLowPass =
                    false;
            }
        }

        localPlayer.bleedingHeavily = false;
        localPlayer.criticallyInjured = false;
        localPlayer.playerBodyAnimator.SetBool("Limp", false);
        localPlayer.health = 100;
        hudManager.UpdateHealthUI(100, false);
        localPlayer.spectatedPlayerScript = null;
        hudManager.audioListenerLowPass.enabled = false;
        startOfRound.SetSpectateCameraToGameOverMode(false, localPlayer);
        var array = Object.FindObjectsOfType<RagdollGrabbableObject>();
        for (var j = 0; j < array.Length; j++)
            if (!array[j].isHeld)
            {
                if (startOfRound.IsServer)
                {
                    if (array[j].NetworkObject.IsSpawned)
                        array[j].NetworkObject.Despawn(true);
                    else
                        Object.Destroy(array[j].gameObject);
                }
            }
            else if (array[j].isHeld && array[j].playerHeldBy != null)
            {
                array[j].playerHeldBy.DropAllHeldItems(true, false);
            }

        var array2 = Object.FindObjectsOfType<DeadBodyInfo>();
        for (var k = 0; k < array2.Length; k++) Object.Destroy(array2[k].gameObject);
        startOfRound.livingPlayers = startOfRound.connectedPlayersAmount + 1;
        startOfRound.allPlayersDead = false;
        startOfRound.UpdatePlayerVoiceEffects();
        startOfRound.shipAnimator.ResetTrigger("ShipLeave");
    }

    private PlayerControllerB HealLocalPlayer(HUDManager hudManager)
    {
        if(Helper.LocalPlayer is not PlayerControllerB localPlayer) return null;
        if (localPlayer.IsDead())
        {
            RespawnLocalPlayer(localPlayer, hudManager.localPlayer.playersManager, hudManager);

            Helper.CreateComponent<WaitForBehaviour>("Respawn")
                .SetPredicate(() => hudManager.localPlayer.playersManager.shipIsLeaving)
                .Init(localPlayer.KillPlayer);
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

    private PlayerControllerB? HealPlayer(string? playerNameOrId)
    {
        var targetPlayer = Helper.GetActivePlayer(playerNameOrId);
        targetPlayer?.HealPlayer();

        return targetPlayer;
    }
}