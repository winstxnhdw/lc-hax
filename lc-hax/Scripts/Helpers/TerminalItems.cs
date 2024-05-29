namespace Hax;

internal static partial class Helper
{
    /// <summary>
    ///     Returns if the door is open or not
    /// </summary>
    /// <param name="door"></param>
    /// <returns></returns>
    internal static bool isDoorOpen(this TerminalAccessibleObject door)
    {
        return door.Reflect().GetInternalField<bool>("isDoorOpen");
    }

    /// <summary>
    ///     This method is used to toggle the door using it's isDoorOpen property.
    /// </summary>
    internal static void ToggleDoor(this TerminalAccessibleObject door)
    {
        door.SetDoor(!door.isDoorOpen());
    }

    /// <summary>
    ///     Opens or closes the Terminal door using RPC
    /// </summary>
    internal static void SetDoor(this TerminalAccessibleObject door, bool isDoorOpen)
    {
        door?.SetDoorOpenServerRpc(isDoorOpen);
    }

    /// <summary>
    ///     Detonates the Landmine
    /// </summary>
    internal static void TriggerMine(this Landmine landmine)
    {
        landmine.Reflect().InvokeInternalMethod("TriggerMineOnLocalClientByExiting");
    }

    /// <summary>
    ///     Returns if the Landmine is active or not
    /// </summary>
    /// <param name="landmine"></param>
    /// <returns></returns>
    internal static bool isLandmineActive(this Landmine landmine)
    {
        return landmine.Reflect().GetInternalField<bool>("mineActivated");
    }

    /// <summary>
    ///     This method is used to toggle the Landmine using it's mineActivated property.
    /// </summary>
    internal static void ToggleLandmine(this Landmine landmine)
    {
        landmine?.SetLandmine(!landmine.isLandmineActive());
    }

    /// <summary>
    ///     This method is used to set the Landmine using RPC
    /// </summary>
    internal static void SetLandmine(this Landmine? landmine, bool mineActivated)
    {
        landmine?.ToggleMineServerRpc(mineActivated);
    }

    /// <summary>
    ///     Returns if a turret is active or not
    /// </summary>
    /// <param name="turret"></param>
    /// <returns></returns>
    internal static bool isTurretActive(this Turret turret)
    {
        return turret.turretActive;
    }

    /// <summary>
    ///     This method is used to toggle the turret using it's TurrectActive property.
    /// </summary>
    internal static void ToggleTurret(this Turret turret)
    {
        turret?.SetTurret(!turret.isTurretActive());
    }

    /// <summary>
    ///     This method is used to set the turret using RPC
    /// </summary>
    internal static void SetTurret(this Turret? turret, bool turretActive)
    {
        turret?.ToggleTurretServerRpc(turretActive);
    }

    /// <summary>
    ///     This makes the Turret go into berserk mode
    /// </summary>
    internal static void BerserkMode(this Turret turret)
    {
        turret?.EnterBerserkModeServerRpc(-1);
    }

    /// <summary>
    ///     Returns if the trap is active or not
    /// </summary>
    /// <param name="spike"></param>
    /// <returns></returns>
    internal static bool isTrapActive(this SpikeRoofTrap spike)
    {
        return spike.trapActive;
    }

    /// <summary>
    ///     This method is used to toggle the spikes using RPC and using it's trapActive property.
    /// </summary>
    /// <param name="spike"></param>
    internal static void ToggleSpikes(this SpikeRoofTrap spike)
    {
        spike?.SetSpikes(!spike.isTrapActive());
    }

    /// <summary>
    ///     This method is used to toggle the spikes using RPC
    /// </summary>
    /// <param name="spike"></param>
    internal static void SetSpikes(this SpikeRoofTrap spike, bool trapActive)
    {
        spike?.ToggleSpikesServerRpc(trapActive);
    }

    /// <summary>
    ///     This method is used to slam the spikes using RPC
    /// </summary>
    /// <param name="spike"></param>
    internal static void Slam(this SpikeRoofTrap spike)
    {
        spike?.SpikeTrapSlamServerRpc(-1);
    }
}