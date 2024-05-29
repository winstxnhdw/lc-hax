using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

internal class HaxObjects : MonoBehaviour
{
    internal static HaxObjects? Instance { get; private set; }

    internal SingleObjectPool<DepositItemsDesk>? DepositItemsDesk { get; private set; }
    internal MultiObjectPool<ShipTeleporter>? ShipTeleporters { get; private set; }
    internal MultiObjectPool<LocalVolumetricFog>? LocalVolumetricFogs { get; private set; }
    internal MultiObjectPool<SteamValveHazard>? SteamValves { get; private set; }
    internal MultiObjectPool<InteractTrigger>? InteractTriggers { get; private set; }

    private void Awake()
    {
        DepositItemsDesk = new SingleObjectPool<DepositItemsDesk>(this, 3.0f);
        ShipTeleporters = new MultiObjectPool<ShipTeleporter>(this);
        LocalVolumetricFogs = new MultiObjectPool<LocalVolumetricFog>(this);
        InteractTriggers = new MultiObjectPool<InteractTrigger>(this);
        SteamValves = new MultiObjectPool<SteamValveHazard>(this, 5.0f);
        Instance = this;
    }
}