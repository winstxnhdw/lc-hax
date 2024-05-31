using UnityEngine.InputSystem.EnhancedTouch;

internal static class TerminalItems
{
    /// <summary>
    /// Determines if the door is open or not.
    /// </summary>
    /// <param name="door">The door object to check.</param>
    /// <returns>True if the door is open; otherwise, false.</returns>
    internal static bool isDoorOpen(this TerminalAccessibleObject door) => door.Reflect().GetInternalField<bool>("isDoorOpen");

    /// <summary>
    /// Toggles the state of the door (open or closed).
    /// </summary>
    /// <param name="door">The door object to toggle.</param>
    internal static void ToggleDoor(this TerminalAccessibleObject door) => door?.SetDoor(!door.isDoorOpen());

    /// <summary>
    /// Sets the state of the door (open or closed) using RPC.
    /// </summary>
    /// <param name="door">The door object to set.</param>
    /// <param name="isDoorOpen">True to open the door; false to close it.</param>
    internal static void SetDoor(this TerminalAccessibleObject door, bool isDoorOpen)
    {
        door.SetDoorOpenServerRpc(isDoorOpen);
    }

    /// <summary>
    /// Detonates the landmine.
    /// </summary>
    /// <param name="landmine">The landmine object to detonate.</param>
    internal static void Explode(this Landmine landmine) => landmine.Reflect().InvokeInternalMethod("TriggerMineOnLocalClientByExiting");

    /// <summary>
    /// Determines if the landmine is active or not.
    /// </summary>
    /// <param name="landmine">The landmine object to check.</param>
    /// <returns>True if the landmine is active; otherwise, false.</returns>
    internal static bool isLandmineActive(this Landmine landmine) => landmine.Reflect().GetInternalField<bool>("mineActivated");

    /// <summary>
    /// Toggles the state of the landmine (active or inactive).
    /// </summary>
    /// <param name="landmine">The landmine object to toggle.</param>
    /// <param name="UseTerminalComponent">Indicates whether to use the terminal component if available.</param>
    internal static void ToggleLandmine(this Landmine landmine, bool UseTerminalComponent = false) => landmine?.SetLandmine(!landmine.isLandmineActive(), UseTerminalComponent);

    /// <summary>
    /// Sets the state of the landmine (active or inactive) using RPC.
    /// </summary>
    /// <param name="landmine">The landmine object to set.</param>
    /// <param name="mineActivated">True to activate the landmine; false to deactivate it.</param>
    /// <param name="UseTerminalComponent">Indicates whether to use the terminal component if available.</param>
    internal static void SetLandmine(this Landmine landmine, bool mineActivated, bool UseTerminalComponent = false)
    {
        if (landmine == null) return;
        if (UseTerminalComponent)
        {
            if (landmine.TryGetComponent(out TerminalAccessibleObject terminal))
            {
                if (!terminal.IsInCooldown())
                {
                    terminal.CallFunctionFromTerminal();
                }

                return;
            }
        }

        landmine?.ToggleMineServerRpc(mineActivated);
    }

    /// <summary>
    /// Determines if the turret is active or not.
    /// </summary>
    /// <param name="turret">The turret object to check.</param>
    /// <returns>True if the turret is active; otherwise, false.</returns>
    internal static bool isTurretActive(this Turret turret) => turret.turretActive;

    /// <summary>
    /// Toggles the state of the turret (active or inactive).
    /// </summary>
    /// <param name="turret">The turret object to toggle.</param>
    /// <param name="UseTerminalComponent">Indicates whether to use the terminal component if available.</param>
    internal static void ToggleTurret(this Turret turret, bool UseTerminalComponent = false) => turret?.SetTurret(!turret.isTurretActive(), UseTerminalComponent);

    /// <summary>
    /// Sets the state of the turret (active or inactive) using RPC.
    /// </summary>
    /// <param name="turret">The turret object to set.</param>
    /// <param name="turretActive">True to activate the turret; false to deactivate it.</param>
    /// <param name="UseTerminalComponent">Indicates whether to use the terminal component if available.</param>
    internal static void SetTurret(this Turret turret, bool turretActive, bool UseTerminalComponent = false)
    {
        if (turret == null) return;
        if (UseTerminalComponent)
        {
            if (turret.TryGetComponent(out TerminalAccessibleObject terminal))
            {
                if (!terminal.IsInCooldown())
                {
                    terminal.CallFunctionFromTerminal();
                }

                return;
            }
        }

        turret?.ToggleTurretServerRpc(turretActive);
    }

    /// <summary>
    /// Makes the turret go into berserk mode.
    /// </summary>
    /// <param name="turret">The turret object to set to berserk mode.</param>
    internal static void Berserk(this Turret turret) => turret?.EnterBerserkModeServerRpc(-1);

    /// <summary>
    /// Determines if the trap is active or not.
    /// </summary>
    /// <param name="spike">The spike trap object to check.</param>
    /// <returns>True if the trap is active; otherwise, false.</returns>
    internal static bool isTrapActive(this SpikeRoofTrap spike) => spike.trapActive;

    /// <summary>
    /// Toggles the state of the spike trap (active or inactive).
    /// </summary>
    /// <param name="spike">The spike trap object to toggle.</param>
    /// <param name="UseTerminalComponent">Indicates whether to use the terminal component if available.</param>
    internal static void ToggleSpikes(this SpikeRoofTrap spike, bool UseTerminalComponent = false) => spike?.SetSpikes(!spike.isTrapActive(), UseTerminalComponent);

    /// <summary>
    /// Sets the state of the spike trap (active or inactive) using RPC.
    /// </summary>
    /// <param name="spike">The spike trap object to set.</param>
    /// <param name="trapActive">True to activate the trap; false to deactivate it.</param>
    /// <param name="UseTerminalComponent">Indicates whether to use the terminal component if available.</param>
    internal static void SetSpikes(this SpikeRoofTrap spike, bool trapActive, bool UseTerminalComponent = false)
    {
        if (spike == null) return;
        if (UseTerminalComponent)
        {
            if (spike.TryGetComponent(out TerminalAccessibleObject terminal))
            {
                if (spike.isTrapActive())
                {
                    if (!terminal.IsInCooldown())
                    {
                        terminal.CallFunctionFromTerminal();
                    }
                }

                return;
            }
        }

        spike?.ToggleSpikesServerRpc(trapActive);
    }

    /// <summary>
    /// Slams the spike trap using RPC.
    /// </summary>
    /// <param name="spike">The spike trap object to slam.</param>
    internal static void Slam(this SpikeRoofTrap spike) => spike?.SpikeTrapSlamServerRpc(-1);

    /// <summary>
    /// Sets the cooldown state of the terminal object.
    /// </summary>
    /// <param name="obj">The terminal object to set cooldown state for.</param>
    /// <param name="inCooldown">True to put the object in cooldown; false to remove it from cooldown.</param>
    internal static void SetInCooldown(this TerminalAccessibleObject obj, bool inCooldown) => obj?.Reflect().SetInternalField("inCooldown", inCooldown);

    /// <summary>
    /// Determines if the terminal object is in cooldown state or not.
    /// </summary>
    /// <param name="obj">The terminal object to check. Can be null.</param>
    /// <returns>True if the terminal object is in cooldown; otherwise, false. Returns false if the object is null.</returns>
    internal static bool IsInCooldown(this TerminalAccessibleObject obj) => obj?.Reflect().GetInternalField<bool>("inCooldown") ?? false;

    /// <summary>
    /// Determines if the spike trap is in cooldown state or not.
    /// </summary>
    /// <param name="spike">The spike trap object to check. Can be null.</param>
    /// <returns>True if the spike trap is in cooldown; otherwise, false. Returns false if the object is null or doesn't have a TerminalAccessibleObject component.</returns>
    internal static bool IsInCooldown(this SpikeRoofTrap spike)
    {
        if (spike.TryGetComponent(out TerminalAccessibleObject terminal))
        {
            return terminal.IsInCooldown();
        }

        return false;
    }

    /// <summary>
    /// Determines if the turret is in cooldown state or not.
    /// </summary>
    /// <param name="turret">The turret object to check. Can be null.</param>
    /// <returns>True if the turret is in cooldown; otherwise, false. Returns false if the object is null or doesn't have a TerminalAccessibleObject component.</returns>
    internal static bool IsInCooldown(this Turret turret)
    {
        if (turret.TryGetComponent(out TerminalAccessibleObject terminal))
        {
            return terminal.IsInCooldown();
        }

        return false;
    }

    /// <summary>
    /// Determines if the landmine is in cooldown state or not.
    /// </summary>
    /// <param name="landmine">The landmine object to check. Can be null.</param>
    /// <returns>True if the landmine is in cooldown; otherwise, false. Returns false if the object is null or doesn't have a TerminalAccessibleObject component.</returns>
    internal static bool IsInCooldown(this Landmine landmine)
    {
        if (landmine.TryGetComponent(out TerminalAccessibleObject terminal))
        {
            return terminal.IsInCooldown();
        }

        return false;
    }



}
