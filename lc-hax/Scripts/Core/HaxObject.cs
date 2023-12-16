#pragma warning disable CS8618

using UnityEngine;

namespace Hax;

public class HaxObject : MonoBehaviour {
    public static HaxObject? Instance { get; private set; }

    public SingleObjectPool<DepositItemsDesk> DepositItemsDesk { get; private set; }
    public MultiObjectPool<Shovel> Shovels { get; private set; }
    public MultiObjectPool<ShipTeleporter> ShipTeleporters { get; private set; }
    public MultiObjectPool<ToggleFogTrigger> ToggleFogTriggers { get; private set; }
    public MultiObjectPool<SteamValveHazard> SteamValves { get; private set; }

    void Awake() {
        this.DepositItemsDesk = new(this);
        this.Shovels = new(this);
        this.ShipTeleporters = new(this);
        this.ToggleFogTriggers = new(this);
        this.SteamValves = new(this, 5.0f);

        HaxObject.Instance = this;
    }
}
