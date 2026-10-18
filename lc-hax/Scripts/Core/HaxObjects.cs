using UnityEngine;

sealed class HaxObjects : MonoBehaviour {
    internal static HaxObjects? Instance { get; private set; }

    internal SingleObjectPool<DepositItemsDesk>? DepositItemsDesk { get; private set; }
    internal MultiObjectPool<ShipTeleporter>? ShipTeleporters { get; private set; }
    internal MultiObjectPool<InteractTrigger>? InteractTriggers { get; private set; }

    void Awake() {
        this.DepositItemsDesk = new(this, 3.0f);
        this.ShipTeleporters = new(this);
        this.InteractTriggers = new(this);
        HaxObjects.Instance = this;
    }
}
