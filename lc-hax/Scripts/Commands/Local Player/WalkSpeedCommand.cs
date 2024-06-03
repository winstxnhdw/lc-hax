#pragma warning disable CS8602

#region

using System;
using GameNetcodeStuff;
using Hax;

#endregion

[Command("walkspeed")]
class WalkSpeedCommand : ICommand {
    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (args.Length == 0 || args[0] == null || args[0].Equals("default", StringComparison.OrdinalIgnoreCase))
            this.Reset(player);
        else if (float.TryParse(args[0], out float speed)) {
            if (speed <= 0)
                this.Reset(player);
            else
                this.SetmovementSpeed(player, speed);
        }
    }

    void SetmovementSpeed(PlayerControllerB player, float Force) {
        if (!Setting.OverrideMovementSpeed) {
            Setting.Default_MovementSpeed = player.movementSpeed;
            Setting.OverrideMovementSpeed = true;
        }

        Setting.New_MovementSpeed = Force;
        player.movementSpeed = Force;
        Helper.SendFlatNotification($"Walk Speed set to {Force}!");
    }

    void Reset(PlayerControllerB player) {
        if (Setting.OverrideMovementSpeed) {
            Setting.New_MovementSpeed = Setting.Default_MovementSpeed;
            player.movementSpeed = Setting.Default_MovementSpeed;
            Setting.OverrideMovementSpeed = false;
            Helper.SendFlatNotification($"Walk Speed reset to default {player.movementSpeed}!");
        }
    }
}
