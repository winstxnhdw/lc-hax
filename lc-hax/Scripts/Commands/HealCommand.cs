#region

using System;
using System.Collections.Generic;
using GameNetcodeStuff;
using Hax;
using Object = UnityEngine.Object;

#endregion

[Command("heal")]
class HealCommand : ICommand, IStun {
    public void Execute(StringArray args) {
        // Handle different cases based on args
        if (args.Length == 0 || args[0].Equals("self", StringComparison.OrdinalIgnoreCase)) {
            if (Helper.LocalPlayer is null) return;
            if (!this.HealPlayer(Helper.LocalPlayer)) Chat.Print("Failed to Heal Self!");

            return;
        }
        else if (args[0].ToLower() == "all") {
            HashSet<string> healedPlayersNames = new HashSet<string>();
            // Heal all players
            foreach (PlayerControllerB? player in Helper.Players) {
                if (player is null) continue;
                string username = player.GetPlayerUsername();
                if (player.IsSelf()) {
                    if (this.HealLocalPlayer(false)) healedPlayersNames.Add(username);
                }
                else {
                    if (player.IsDead()) continue;
                    if (this.HealPlayer(player)) healedPlayersNames.Add(username);
                }
            }

            Helper.SendFlatNotification($"Healed: {string.Join(", ", healedPlayersNames)}");
            return;
        }
        else
            this.HealPlayer(string.Join(" ", args));
    }

    void RespawnLocalPlayer() {
        if (Helper.SoundManager is not SoundManager soundManager) return;
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (Helper.HUDManager is not HUDManager hudManager) return;
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;

        startOfRound.allPlayersDead = false;
        localPlayer.ResetPlayerBloodObjects(localPlayer.IsDead());
        if (localPlayer.IsDeadAndNotControlled()) {
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
            localPlayer.DisablePlayerModel(startOfRound.allPlayerObjects[localPlayer.GetPlayerId()], true, true);
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
            soundManager.SetPlayerPitch(1f, (int)localPlayer.GetPlayerId());
            if (localPlayer.currentVoiceChatIngameSettings == null) startOfRound.RefreshPlayerVoicePlaybackObjects();
            if (localPlayer.currentVoiceChatIngameSettings != null) {
                if (localPlayer.currentVoiceChatIngameSettings.voiceAudio == null)
                    localPlayer.currentVoiceChatIngameSettings.InitializeComponents();
                if (localPlayer.currentVoiceChatIngameSettings.voiceAudio != null)
                    if (localPlayer.currentVoiceChatIngameSettings.voiceAudio.TryGetComponent<OccludeAudio>(
                            out OccludeAudio? occludeAudio))
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
        RagdollGrabbableObject[]? array = Object.FindObjectsOfType<RagdollGrabbableObject>();
        for (int j = 0; j < array.Length; j++)
            if (!array[j].isHeld) {
                if (startOfRound.IsServer) {
                    if (array[j].NetworkObject.IsSpawned)
                        array[j].NetworkObject.Despawn(true);
                    else
                        Object.Destroy(array[j].gameObject);
                }
            }
            else if (array[j].isHeld && array[j].playerHeldBy != null)
                array[j].playerHeldBy.DropAllHeldItems(true, false);

        DeadBodyInfo[]? array2 = Object.FindObjectsOfType<DeadBodyInfo>();
        for (int k = 0; k < array2.Length; k++) Object.Destroy(array2[k].gameObject);
        startOfRound.livingPlayers = startOfRound.connectedPlayersAmount + 1;
        startOfRound.allPlayersDead = false;
        startOfRound.UpdatePlayerVoiceEffects();
        startOfRound.shipAnimator.ResetTrigger("ShipLeave");
    }

    bool HealLocalPlayer(bool revive = false) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return false;
        if (Helper.HUDManager is not HUDManager hudManager) return false;
        if (localPlayer.IsDead() && revive) {
            this.RespawnLocalPlayer();

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
        Helper.SendFlatNotification("You got Healed!");
        return true;
    }

    void HealPlayer(string? playerNameOrId) {
        PlayerControllerB? targetPlayer = Helper.GetPlayer(playerNameOrId);
        if (targetPlayer == null) {
            Helper.SendFlatNotification($"Player {playerNameOrId} not found!");
            return;
        }

        this.HealPlayer(targetPlayer);
    }

    bool HealPlayer(PlayerControllerB player) {
        if (player is null) return false;
        string username = player.GetPlayerUsername();
        bool Healed = false;
        if (player.IsSelf()) {
            this.HealLocalPlayer(true);
            Healed = true;
        }
        else {
            if (!player.IsDead()) {
                player?.HealPlayer();
                Healed = true;
            }
        }

        if (Healed)
            this.Stun(player.transform.position, 5.0f, 1.0f);
        else {
            Helper.SendFlatNotification(!player.IsDead()
                ? $"Failed to heal {username}!"
                : $"Failed to Heal {username} : DEAD PLAYER");
        }

        return Healed;
    }
}
