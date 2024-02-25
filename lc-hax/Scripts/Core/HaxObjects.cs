using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

class HaxObjects : MonoBehaviour {
    internal static HaxObjects? Instance { get; private set; }

    internal SingleObjectPool<DepositItemsDesk>? DepositItemsDesk { get; private set; }
    internal MultiObjectPool<ShipTeleporter>? ShipTeleporters { get; private set; }
    internal MultiObjectPool<LocalVolumetricFog>? LocalVolumetricFogs { get; private set; }
    internal MultiObjectPool<SteamValveHazard>? SteamValves { get; private set; }
    internal MultiObjectPool<InteractTrigger>? InteractTriggers { get; private set; }

    void Awake() {
        this.DepositItemsDesk = new(this, 3.0f);
        this.ShipTeleporters = new(this);
        this.LocalVolumetricFogs = new(this);
        this.InteractTriggers = new(this);
        this.SteamValves = new(this, 5.0f);
        HaxObjects.Instance = this;
    }
}
