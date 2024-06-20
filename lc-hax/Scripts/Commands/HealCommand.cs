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

    

    bool HealLocalPlayer(bool revive = false) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return false;
        if (Helper.HUDManager is not HUDManager hudManager) return false;
        if (localPlayer.IsDeadAndNotControlled() && revive) {
            Helper.RespawnLocalPlayer();

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
