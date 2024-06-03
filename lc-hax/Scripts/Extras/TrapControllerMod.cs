#region

using System;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

#endregion

sealed class TrapControllerMod : MonoBehaviour {
    internal bool OnlyForLocalPlayer = true;
    internal bool useTerminalToControl = true;
    internal string TrapType { get; private set; }
    internal float Radius { get; private set; } = 5f;

    Turret Turret { get; set; }
    SpikeRoofTrap SpikeRoofTrap { get; set; }
    Landmine Landmine { get; set; }

    internal void Start() {
        // if there is no Turret, SpikeRoofTrap or Landmine component, destroy this script
        if (!this.gameObject.GetComponent<Turret>() && !this.gameObject.GetComponent<SpikeRoofTrap>() &&
            !this.gameObject.GetComponent<Landmine>())
            Destroy(this);

        // else parse one of these 3 components
        if (this.gameObject.GetComponent<SpikeRoofTrap>()) {
            this.SpikeRoofTrap = this.gameObject.GetComponent<SpikeRoofTrap>();
            this.TrapType = "SpikeRoofTrap";
            this.Radius = 13;
        }
        else if (this.gameObject.GetComponent<Landmine>()) {
            this.Landmine = this.gameObject.GetComponent<Landmine>();
            this.TrapType = "Landmine";
            this.Radius = 8;
        }
        else if (this.gameObject.GetComponent<Turret>()) {
            this.Turret = this.gameObject.GetComponent<Turret>();
            this.TrapType = "Turret";
            this.Radius = 10;
        }
    }

    void Update() => this.CheckForPlayerNearby();

    void CheckForPlayerNearby() {
        if (this.isTrapActive()) {
            Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, this.Radius, Mask.Player);
            foreach (Collider hitCollider in hitColliders) {
                PlayerControllerB? player = hitCollider.GetComponent<PlayerControllerB>();
                if (player != null) {
                    if (this.OnlyForLocalPlayer && !player.IsSelf()) return;
                    if (!this.isTrapInCooldown()) {
                        this.ToggleTrap(false, this.useTerminalToControl);
                        Console.WriteLine(
                            $"{this.TrapType} Detected {player.playerUsername}'s nearby, Turning trap off.");
                    }

                    break;
                }
            }
        }
    }

    internal bool isTrapActive() {
        if (this.SpikeRoofTrap != null)
            return this.SpikeRoofTrap.isTrapActive();
        else if (this.Landmine != null)
            return this.Landmine.isLandmineActive();
        else if (this.Turret != null) return this.Turret.isTurretActive();
        return false;
    }

    internal bool isTrapInCooldown() {
        if (this.useTerminalToControl) {
            if (this.SpikeRoofTrap != null)
                return this.SpikeRoofTrap.IsInCooldown();
            else if (this.Landmine != null)
                return this.Landmine.IsInCooldown();
            else if (this.Turret != null) return this.Turret.IsInCooldown();
        }

        return false;
    }

    internal void ToggleTrap(bool Active, bool UseTerminal) {
        if (this.SpikeRoofTrap != null) this.SpikeRoofTrap.SetSpikes(Active, UseTerminal);
        if (this.Landmine != null) this.Landmine.SetLandmine(Active, UseTerminal);
        if (this.Turret != null) this.Turret.SetTurret(Active, UseTerminal);
    }
}
