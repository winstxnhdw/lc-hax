#pragma warning disable CS8618

using UnityEngine;

namespace Hax;

public class HaxObjects : MonoBehaviour {
    public static HaxObjects? Instance { get; private set; }

    public MultiObjectPool<Shovel> Shovels { get; private set; }
    public MultiObjectPool<ShipTeleporter> ShipTeleporters { get; private set; }

    void Awake() {
        this.Shovels = new(this);
        this.ShipTeleporters = new(this);

        HaxObjects.Instance = this;
    }
}
