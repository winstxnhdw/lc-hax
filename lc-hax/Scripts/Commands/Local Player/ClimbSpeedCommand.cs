#pragma warning disable CS8602

using System;
using GameNetcodeStuff;
using Hax;

[Command("climbspeed")]
internal class ClimbSpeedCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (args.Length == 0 || args[0] == null || args[0].Equals("default", StringComparison.OrdinalIgnoreCase))
        {
            Reset(player);
        }
        else if (float.TryParse(args[0], out var speed))
        {
            if (speed <= 0)
                Reset(player);
            else
                SetClimbSpeed(player, speed);
        }
    }

    private void SetClimbSpeed(PlayerControllerB player, float Force)
    {
        if (!Setting.OverrideClimbSpeed)
        {
            Setting.Default_climbSpeed = player.climbSpeed;
            Setting.OverrideClimbSpeed = true;
        }

        Setting.New_ClimbSpeed = Force;
        player.climbSpeed = Force;
        Helper.SendFlatNotification($"Climb Speed set to {Force}!");
    }

    private void Reset(PlayerControllerB player)
    {
        if (Setting.OverrideClimbSpeed)
        {
            Setting.New_ClimbSpeed = Setting.Default_climbSpeed;
            player.climbSpeed = Setting.Default_climbSpeed;
            Setting.OverrideClimbSpeed = false;
            Helper.SendFlatNotification($"Climb Speed reset to default {player.climbSpeed}!");
        }
    }
}