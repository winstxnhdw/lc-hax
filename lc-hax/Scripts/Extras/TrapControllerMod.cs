using System;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

internal sealed class TrapControllerMod : MonoBehaviour
{
    internal bool DisableTrapWhenPlayerNearby = true;

    private bool HasTrapBeenDisabled = false;
    private float nextToggleTime = 0.0f;
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
        if (!HasTrapBeenDisabled)
        {
            CheckForPlayerNearby();
        }
        else
        {
            // reactivate the trap after 10 seconds after it has been disabled
            if (Time.time >= nextToggleTime)
            {
                ToggleTrap(true);
                Console.WriteLine($"{TrapType} has been reactivated.");
                HasTrapBeenDisabled = false;
                nextToggleTime = 0;
            }
        }
    }

    private void CheckForPlayerNearby()
    {
        if (DisableTrapWhenPlayerNearby)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius, Mask.Player);
            foreach (var hitCollider in hitColliders)
            {
                var player = hitCollider.GetComponent<PlayerControllerB>();
                if (player != null)
                {
                    if (OnlyForLocalPlayer && !player.IsSelf()) return;
                    if (isTrapActive())
                    {
                        ToggleTrap(false);
                        Console.WriteLine($"{TrapType} Detected {player.playerUsername}'s nearby, Turning trap off.");
                        HasTrapBeenDisabled = true;
                        // add a delay of 10 seconds before reactivating the trap
                        nextToggleTime = Time.time + 10f;
                        break;
                    }
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

    internal void ToggleTrap(bool Active)
    {
        if (SpikeRoofTrap != null) SpikeRoofTrap.SetSpikes(Active);
        if (Landmine != null) Landmine.SetLandmine(Active);
        if (Turret != null) Turret.SetTurret(Active);
    }
}