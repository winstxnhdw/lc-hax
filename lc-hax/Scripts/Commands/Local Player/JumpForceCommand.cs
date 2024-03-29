#pragma warning disable CS8602 

using System;
using GameNetcodeStuff;
using Hax;

[Command("jumpforce")]
internal class JumpForceCommand : ICommand {
    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (args.Length == 0 || args[0] == null || args[0].Equals("default", StringComparison.OrdinalIgnoreCase)) {
            this.Reset(player);
        }
        else if (float.TryParse(args[0], out float speed)) {
            if (speed <= 0) {
                this.Reset(player);
            }
            else {
                this.SetJumpForce(player, speed);
            }
        }
    }

   void SetJumpForce(PlayerControllerB player, float Force) {
        if (!Setting.OverrideJumpForce) {
            Setting.Default_JumpForce = player.jumpForce;
            Setting.OverrideJumpForce = true;
        }
        Setting.New_JumpForce = Force;
        player.jumpForce = Force;
        Helper.SendNotification("Jump Force", $"Jump Force set to {Force}!");
    }

    void Reset(PlayerControllerB player) {
        if (Setting.OverrideJumpForce) {
            Setting.New_JumpForce = Setting.Default_JumpForce;
            player.jumpForce = Setting.Default_JumpForce;
            Setting.OverrideJumpForce = false;
            Helper.SendNotification("Jump Force", $"Jump Force reset to default {player.jumpForce}!");
        }

    }


}
