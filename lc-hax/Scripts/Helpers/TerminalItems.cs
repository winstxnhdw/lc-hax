namespace Hax;

static partial class Helper {

    /// <summary>
    /// This method is used to toggle the door using it's isDoorOpen property.
    /// </summary>
    internal static void ToggleDoor(this TerminalAccessibleObject door) => door?.SetDoorOpen(!door.Reflect().GetInternalField<bool>("isDoorOpen"));

    /// <summary>
    /// Opens or closes the Terminal door using RPC
    /// </summary>
    internal static void SetDoorOpen(this TerminalAccessibleObject door, bool isDoorOpen) => door.SetDoorOpenServerRpc(isDoorOpen);

    /// <summary>
    /// Detonates the Landmine
    /// </summary>
    internal static void TriggerMine(this Landmine landmine) => landmine.Reflect().InvokeInternalMethod("TriggerMineOnLocalClientByExiting");

    /// <summary>
    /// This method is used to toggle the Landmine using it's mineActivated property.
    /// </summary>
    internal static void ToggleLandmine(this Landmine landmine) => landmine?.SetLandmine(!landmine.Reflect().GetInternalField<bool>("mineActivated"));
    /// <summary>
    /// This method is used to set the Landmine using RPC
    /// </summary>
    internal static void SetLandmine(this Landmine? landmine, bool mineActivated) => landmine?.ToggleMineServerRpc(mineActivated);

    /// <summary>
    /// This method is used to toggle the turret using it's TurrectActive property.
    /// </summary>
    internal static void ToggleTurret(this Turret turret) => turret?.SetTurret(!turret.turretActive);
    /// <summary>
    /// This method is used to set the turret using RPC
    /// </summary>
    internal static void SetTurret(this Turret? turret, bool turretActive) => turret?.ToggleTurretClientRpc(turretActive);

    /// <summary>
    /// This makes the Turret go into berserk mode
    /// </summary>
    internal static void BerserkMode(this Turret turret) => turret?.EnterBerserkModeServerRpc(-1);


}
