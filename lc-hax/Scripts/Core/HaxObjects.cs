using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class HaxObjects : MonoBehaviour {
    public static HaxObjects? Instance { get; private set; }

    public SingleObjectPool<DepositItemsDesk>? DepositItemsDesk { get; private set; }
    public MultiObjectPool<ShipTeleporter>? ShipTeleporters { get; private set; }
    public MultiObjectPool<LocalVolumetricFog>? LocalVolumetricFogs { get; private set; }
    public MultiObjectPool<SteamValveHazard>? SteamValves { get; private set; }
    public MultiObjectPool<InteractTrigger>? InteractTriggers { get; private set; }

    void Awake() {
        this.DepositItemsDesk = new(this, 3.0f);
        this.ShipTeleporters = new(this);
        this.LocalVolumetricFogs = new(this);
        this.InteractTriggers = new(this);
        this.SteamValves = new(this, 5.0f);
        HaxObjects.Instance = this;
    }
}
