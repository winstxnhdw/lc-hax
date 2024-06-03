#region

using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

#endregion

class HaxObjects : MonoBehaviour {
    internal static HaxObjects? Instance { get; private set; }

    internal SingleObjectPool<DepositItemsDesk>? DepositItemsDesk { get; private set; }
    internal MultiObjectPool<ShipTeleporter>? ShipTeleporters { get; private set; }
    internal MultiObjectPool<LocalVolumetricFog>? LocalVolumetricFogs { get; private set; }
    internal MultiObjectPool<SteamValveHazard>? SteamValves { get; private set; }
    internal MultiObjectPool<InteractTrigger>? InteractTriggers { get; private set; }

    void Awake() {
        this.DepositItemsDesk = new SingleObjectPool<DepositItemsDesk>(this, 3.0f);
        this.ShipTeleporters = new MultiObjectPool<ShipTeleporter>(this);
        this.LocalVolumetricFogs = new MultiObjectPool<LocalVolumetricFog>(this);
        this.InteractTriggers = new MultiObjectPool<InteractTrigger>(this);
        this.SteamValves = new MultiObjectPool<SteamValveHazard>(this, 5.0f);
        Instance = this;
    }
}
