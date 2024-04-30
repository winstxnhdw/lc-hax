using GameNetcodeStuff;
using Hax;
using System;
using UnityEngine;

internal sealed class TrapControllerMod : MonoBehaviour
{
    internal string TrapType { get; private set; }
    internal float Radius { get; private set; } = 5f;

    internal bool DisableTrapWhenPlayerNearby = true;
    internal bool OnlyForLocalPlayer = true;

    private Turret Turret { get; set; }
    private SpikeRoofTrap SpikeRoofTrap { get; set; }
    private Landmine Landmine { get; set; }

    private bool HasTrapBeenDisabled = false;
    float nextToggleTime = 0.0f;

    internal void Start()
    {
        // if there is no Turret, SpikeRoofTrap or Landmine component, destroy this script
        if (!this.gameObject.GetComponent<Turret>() && !this.gameObject.GetComponent<SpikeRoofTrap>() && !this.gameObject.GetComponent<Landmine>())
            Destroy(this);

        // else parse one of these 3 components
        if (this.gameObject.GetComponent<SpikeRoofTrap>())
        {
            SpikeRoofTrap = this.gameObject.GetComponent<SpikeRoofTrap>();
            TrapType = "SpikeRoofTrap";
            Radius = 40f;
        }
        else if (this.gameObject.GetComponent<Landmine>())
        {
            Landmine = this.gameObject.GetComponent<Landmine>();
            TrapType = "Landmine";
            Radius = 15f;
        }
        else if (this.gameObject.GetComponent<Turret>())
        {
            Turret = this.gameObject.GetComponent<Turret>();
            TrapType = "Turret";
            Radius = 35f;
        }
    }

    private void Update()
    {
        if(!HasTrapBeenDisabled)
        {
            CheckForPlayerNearby();
        }
        else
        {
            // reactivate the trap after 10 seconds after it has been disabled
            if(Time.time >= nextToggleTime)
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
            Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 5f, Mask.Player);
            foreach (Collider hitCollider in hitColliders)
            {
                PlayerControllerB player = hitCollider.GetComponent<PlayerControllerB>();
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
        {
            return SpikeRoofTrap.isTrapActive();
        }
        else if (Landmine != null)
        {
            return Landmine.isLandmineActive();
        }
        else if (Turret != null)
        {
            return Turret.isTurretActive();
        }
        return false;
    }

    internal void ToggleTrap(bool Active)
    {
        if (SpikeRoofTrap != null)
        {
            if (SpikeRoofTrap.isTrapActive() != Active)
            {
                SpikeRoofTrap.SetSpikes(Active);
            }
        }
        else if (Landmine != null)
        {
            if (Landmine.isLandmineActive() != Active)
            {
                Landmine.SetLandmine(Active);
            }
        }
        else if (Turret != null)
        {
            if (Turret.isTurretActive() != Active)
            {
                Turret.SetTurret(Active);
            }
        }
    }
}