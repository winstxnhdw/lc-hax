using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class HaxObjects : MonoBehaviour {
    public static HaxObjects? Instance { get; private set; }

    public SingleObjectPool<GameNetworkManager> GameNetworkManager { get; private set; } = null!;
    public SingleObjectPool<ScanNodeProperties> ScanNodeProperties { get; private set; } = null!;
    public SingleObjectPool<HUDManager> HUDManager { get; private set; } = null!;

    public MultiObjectPool<PlayerControllerB> Players { get; private set; } = null!;
    public MultiObjectPool<Shovel> Shovels { get; private set; } = null!;

    void Awake() {
        this.GameNetworkManager = new(this);
        this.ScanNodeProperties = new(this);
        this.HUDManager = new(this);

        this.Players = new(this);
        this.Shovels = new(this);

        HaxObjects.Instance = this;
    }
}
