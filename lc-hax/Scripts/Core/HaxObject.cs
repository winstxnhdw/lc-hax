#pragma warning disable CS8618

using UnityEngine;

namespace Hax;

public class HaxObject : MonoBehaviour {
    public static HaxObject? Instance { get; private set; }

    public MultiObjectPool<Shovel> Shovels { get; private set; }
    public MultiObjectPool<ShipTeleporter> ShipTeleporters { get; private set; }

    void Awake() {
        this.Shovels = new(this);
        this.ShipTeleporters = new(this);

        HaxObject.Instance = this;
    }
}
