#pragma warning disable CS8618

using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class HaxObjects : MonoBehaviour {
    public static HaxObjects? Instance { get; private set; }

    public SingleObjectPool<GameNetworkManager> GameNetworkManager { get; private set; }
    public SingleObjectPool<ScanNodeProperties> ScanNodeProperties { get; private set; }
    public SingleObjectPool<HUDManager> HUDManager { get; private set; }
    public SingleObjectPool<Terminal> Terminal { get; private set; }

    public MultiObjectPool<PlayerControllerB> Players { get; private set; }
    public MultiObjectPool<Shovel> Shovels { get; private set; }

    void Awake() {
        this.GameNetworkManager = new(this);
        this.ScanNodeProperties = new(this);
        this.HUDManager = new(this);
        this.Terminal = new(this);

        this.Players = new(this);
        this.Shovels = new(this);

        HaxObjects.Instance = this;
    }
}
