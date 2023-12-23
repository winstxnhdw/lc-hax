#pragma warning disable CS8618

using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Hax;

public class HaxObjects : MonoBehaviour {
    public static HaxObjects? Instance { get; private set; }

    public SingleObjectPool<DepositItemsDesk> DepositItemsDesk { get; private set; }
    public MultiObjectPool<ShipTeleporter> ShipTeleporters { get; private set; }
    public MultiObjectPool<LocalVolumetricFog> LocalVolumetricFogs { get; private set; }
    public MultiObjectPool<SteamValveHazard> SteamValves { get; private set; }

    void Awake() {
        this.DepositItemsDesk = new(this);
        this.ShipTeleporters = new(this);
        this.LocalVolumetricFogs = new(this);
        this.SteamValves = new(this, 5.0f);

        HaxObjects.Instance = this;
    }
}
