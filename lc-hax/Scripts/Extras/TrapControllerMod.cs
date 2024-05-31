using GameNetcodeStuff;
using Hax;
using System;
using UnityEngine;

internal sealed class TrapControllerMod : MonoBehaviour
{
    internal bool UseTerminalToControl = true;

    internal bool OnlyForLocalPlayer = true;
    internal string TrapType { get; private set; }
    internal float Radius { get; private set; } = 5f;

    private Turret Turret { get; set; }
    private SpikeRoofTrap SpikeRoofTrap { get; set; }
    private Landmine Landmine { get; set; }

    internal void Start()
    {
        // if there is no Turret, SpikeRoofTrap or Landmine component, destroy this script
        if (!gameObject.GetComponent<Turret>() && !gameObject.GetComponent<SpikeRoofTrap>() &&
            !gameObject.GetComponent<Landmine>())
            Destroy(this);

        // else parse one of these 3 components
        if (gameObject.GetComponent<SpikeRoofTrap>())
        {
            SpikeRoofTrap = gameObject.GetComponent<SpikeRoofTrap>();
            TrapType = "SpikeRoofTrap";
            Radius = 13;
        }
        else if (gameObject.GetComponent<Landmine>())
        {
            Landmine = gameObject.GetComponent<Landmine>();
            TrapType = "Landmine";
            Radius = 8;
        }
        else if (gameObject.GetComponent<Turret>())
        {
            Turret = gameObject.GetComponent<Turret>();
            TrapType = "Turret";
            Radius = 10;
        }
    }

    private void Update()
    {
        CheckForPlayerNearby();
    }

    private void CheckForPlayerNearby()
    {
        if (isTrapActive())
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius, Mask.Player);
            foreach (var hitCollider in hitColliders)
            {
                var player = hitCollider.GetComponent<PlayerControllerB>();
                if (player != null)
                {
                    if (OnlyForLocalPlayer && !player.IsSelf()) return;
                    if (!isTrapInCooldown())
                    {
                        ToggleTrap(false, UseTerminalToControl);
                        Console.WriteLine($"{TrapType} Detected {player.playerUsername}'s nearby, Turning trap off.");
                    }
                    break;
                }
            }
        }
    }

    internal bool isTrapActive()
    {
        if (SpikeRoofTrap != null)
            return SpikeRoofTrap.isTrapActive();
        else if (Landmine != null)
            return Landmine.isLandmineActive();
        else if (Turret != null) return Turret.isTurretActive();
        return false;
    }

    internal bool isTrapInCooldown()
    {
        if (UseTerminalToControl)
        {
            if (SpikeRoofTrap != null)
                return SpikeRoofTrap.IsInCooldown();
            else if (Landmine != null)
                return Landmine.IsInCooldown();
            else if (Turret != null) return Turret.IsInCooldown();
        }
        return false;
    }

    internal void ToggleTrap(bool Active, bool UseTerminal)
    {
        if (SpikeRoofTrap != null) SpikeRoofTrap.SetSpikes(Active, UseTerminal);
        if (Landmine != null) Landmine.SetLandmine(Active, UseTerminal);
        if (Turret != null) Turret.SetTurret(Active, UseTerminal);
    }
}