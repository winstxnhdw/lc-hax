#region

using System;
using System.Collections.Generic;
using GameNetcodeStuff;
using Hax;
using Object = UnityEngine.Object;

#endregion

[Command("heal")]
internal class HealCommand : ICommand, IStun
{
    public void Execute(StringArray args)
    {
        // Handle different cases based on args
        if (args.Length == 0 || args[0].Equals("self", StringComparison.OrdinalIgnoreCase))
        {
            if (Helper.LocalPlayer is null) return;
            if (!HealPlayer(Helper.LocalPlayer)) Chat.Print("Failed to heal the local player!");

            return;
        }
        else if (args[0].ToLower() == "all")
        {
            var healedPlayersNames = new HashSet<string>();
            // Heal all players
            foreach (var player in Helper.Players)
            {
                if (player is null) continue;
                var username = player.GetPlayerUsername();
                if (player.IsSelf())
                {
                    if (HealLocalPlayer(false)) healedPlayersNames.Add(username);
                }
                else
                {
                    if (player.IsDead()) continue;
                    if (HealPlayer(player)) healedPlayersNames.Add(username);
                }
            }

            Helper.SendFlatNotification($"Healed: {string.Join(", ", healedPlayersNames)}");
            return;
        }
        else
        {
            HealPlayer(string.Join(" ", args));
        }
    }

    private void RespawnLocalPlayer()
    {
        if (Helper.SoundManager is not SoundManager soundManager) return;
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (Helper.HUDManager is not HUDManager hudManager) return;
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        
        startOfRound.allPlayersDead = false;
        localPlayer.ResetPlayerBloodObjects(localPlayer.IsDead());
        if (localPlayer.isDeadAndNotControlled())
        {
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
            localPlayer.DisablePlayerModel(startOfRound.allPlayerObjects[localPlayer.GetPlayerID()], true, true);
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
            hudManager.gasHelmetAnimator.SetBool("gasEmitting", false);
            localPlayer.hasBegunSpectating = false;
            hudManager.RemoveSpectateUI();
            hudManager.gameOverAnimator.SetTrigger("revive");
            localPlayer.hinderedMultiplier = 1f;
            localPlayer.isMovementHindered = 0;
            localPlayer.sourcesCausingSinking = 0;
            localPlayer.reverbPreset = startOfRound.shipReverb;
            soundManager.earsRingingTimer = 0f;
            localPlayer.voiceMuffledByEnemy = false;
            soundManager.playerVoicePitchTargets[localPlayer.GetPlayerID_ULong()] = 1f;
            soundManager.SetPlayerPitch(1f, (int)localPlayer.GetPlayerID());
            if (localPlayer.currentVoiceChatIngameSettings == null) startOfRound.RefreshPlayerVoicePlaybackObjects();
            if (localPlayer.currentVoiceChatIngameSettings != null)
            {
                if (localPlayer.currentVoiceChatIngameSettings.voiceAudio == null)
                    localPlayer.currentVoiceChatIngameSettings.InitializeComponents();
                if (localPlayer.currentVoiceChatIngameSettings.voiceAudio != null)
                    if (localPlayer.currentVoiceChatIngameSettings.voiceAudio.TryGetComponent<OccludeAudio>(
                            out var occludeAudio))
                        occludeAudio.overridingLowPass = false;
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

    private bool HealLocalPlayer(bool revive = false)
    {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return false;
        if (Helper.HUDManager is not HUDManager hudManager) return false;
        if (localPlayer.IsDead() && revive)
        {
            RespawnLocalPlayer();

            Helper.CreateComponent<WaitForBehaviour>("Respawn")
                .SetPredicate(() => hudManager.localPlayer.playersManager.shipIsLeaving)
                .Init(localPlayer.KillPlayer);
            Helper.SendFlatNotification("You got Respawned Locally (You will die once ship leaves)");
            return true;
        }

        hudManager.localPlayer.health = 100;
        hudManager.localPlayer.bleedingHeavily = false;
        hudManager.localPlayer.criticallyInjured = false;
        hudManager.localPlayer.hasBeenCriticallyInjured = false;
        hudManager.localPlayer.playerBodyAnimator.SetBool("Limp", false);
        hudManager.HUDAnimator.SetBool("biohazardDamage", false);
        hudManager.HUDAnimator.SetTrigger("HealFromCritical");
        hudManager.UpdateHealthUI(hudManager.localPlayer.health, false);
        return true;
    }

    private bool HealPlayer(string? playerNameOrId)
    {
        var targetPlayer = Helper.GetPlayer(playerNameOrId);
        if (targetPlayer == null)
        {
            Helper.SendFlatNotification($"Player {playerNameOrId} not found!");
            return false;
        }

        return HealPlayer(targetPlayer);
    }

    private bool HealPlayer(PlayerControllerB? player)
    {
        if (player is null) return false;
        var username = player.GetPlayerUsername();
        if (player.IsSelf())
        {
            HealLocalPlayer(true);
            this.Stun(player.transform.position, 5.0f, 1.0f);
            return true;
        }
        else
        {
            if (!player.IsDead())
            {
                player?.HealPlayer();
                this.Stun(player.transform.position, 5.0f, 1.0f);
                return true;
            }
            else
            {
                if (username != null) Helper.SendFlatNotification($"Cannot Heal {username} (DEAD)");
            }
        }

        return false;
    }
}